\ Example IR receiver
\ with TSOP1838 on PA2 with nec protocol
\ be aware I just packed the bits in IRCODE how I like them

\ needs io.fs from jcw
\ made for STM32F103C8 'blue pill'

\ data from RM0008 for STM32F10xxx
\ $40010000 constant afio
\ 4 bit defining the port PA -> %0000 PB-> %0001 ...
\ starting with exti0
afio $08 + constant EXTICR1
\ starting with exti5
\ afio $0c + constant EXTICR2
\ starting with exti8
\ afio $10 + constant EXTICR3
\ starting with exti12
\ afio $14 + constant EXTICR4

\ bits for enabling exti
afio $400 + constant EXTI_IMR
\ bits for raising edge irq
\ afio $408 + constant EXTI_RTSR
\ bits for falling edge irq
afio $40C + constant EXTI_FTSR
\ irq status, also for irq reset
afio $414 + constant EXTI_PR

\ documented in RM0008 for STM32F10xxx
\ table 61 Vector table for connectivity line devices
\ from position 32 it starts with NVIC_ISER1
\ $E000E100 constant NVIC_ISER0 
\ NVIC_ISER0 $4 + constant NVIC_ISER1 
\ 6 constant EXTI0_irq 
\ 7 constant EXTI1_irq 
8 constant EXTI2_irq

0 variable IRTime
0 variable IRBIT
0 variable IRCODE

: IR.
    CR IRCODE @ hex. \ just print out the code
;

\ irq service routine for IR input
\ very simple not much error prone, but works fro me :)
: ir_isr
    2 bit EXTI_PR bit@ not if exit then  \ exit wenn nicht exti0
    micros IRTime @ over IRTIME ! - \ get diff to last irq
    case
	dup 13500 - abs 2000 < ?of
	    0 IRBIT !
	    0 IRCODE !
        endof
	dup 1200 - abs 200 < ?of
	    1 IRBIT +!		\ its L, next one
	endof
	dup 2300 - abs 200 < ?of
	    IRBIT @ 16 + 32 mod bit
	    IRCODE bis! \ set it high
	    1 IRBIT +!
	endof
	\ ." E" dup .
    endcase
    IRBIT @ 32 = if \ yeah all bits arrived
	IR.
    then
    2 bit EXTI_PR bis! \ clear exti0
;

\ setup code for only PA2
: irsetup
    imode-float PA2 io-mode! \ for IR TSOP1838 sensor
    ['] ir_isr irq-exti2 ! \ set isr for exti2
    
    $0F00 EXTICR1 bic!   \ PA2 for exti2
    2 bit EXTI_FTSR bis! \ falling edge for exti2
    2 bit EXTI_IMR bis!  \ enable exti2
    
    EXTI2_irq bit nvic_iser0 bis! \ enable exti2 in nvic
    CR ." IR Ready!"
;
    
