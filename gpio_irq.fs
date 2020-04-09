\ Example using simple interupt driven buttons or touch sensors
\ not debounced...
\ needs io.fs from jcw
\ made for STM32F103C8 'blue pill'

\ data from RM0008 for STM32F10xxx
\ $40010000 constant afio
\ 4 bit defining the port PA -> %0000 PB-> %0001 ...
\ starting with exti0
afio $08 + constant EXTICR1
\ starting with exti5
afio $0c + constant EXTICR2
\ starting with exti8
afio $10 + constant EXTICR3
\ starting with exti12
afio $14 + constant EXTICR4

\ bits for enabling exti
afio $400 + constant EXTI_IMR
\ bits for raising edge irq
afio $408 + constant EXTI_RTSR
\ bits for falling edge irq
afio $40C + constant EXTI_FTSR
\ irq status, also for irq reset
afio $414 + constant EXTI_PR

\ documented in RM0008 for STM32F10xxx
\ table 61 Vector table for connectivity line devices
\ from position 32 it starts with NVIC_ISER1
\ $E000E100 constant NVIC_ISER0 
\ NVIC_ISER0 $4 + constant NVIC_ISER1 
6 constant EXTI0_irq 
23 constant EXTI5_irq



\ irq service routine
: gpio_isr
    EXTI_PR @ case \ irq status case
	dup 0 bit and ?of \ check for exti0
	    CR ." PA0"
	    0 bit EXTI_PR bis! \ clear exti0
        endof
	dup 5 bit and ?of \ check for exti5
	    CR ." PB5"
	    5 bit EXTI_PR bis! \ clear exti5
	endof
        dup 8 bit and ?of \ check for exti8
	    CR ." PB8"
	    8 bit EXTI_PR bis! \ clear exti8
        endof
    endcase
;

\ setup code for only PA0
: setup_PA0
    imode-float PA0 io-mode! \ for touch sensor
\    imode-pull PA0 io-mode! \ most other things
    

    ['] gpio_isr irq-exti0 ! \ set isr for exti0
    
    $000F EXTICR1 bic!   \ PA0 for exti0
    0 bit EXTI_RTSR bis! \ rising edge for exti0
\    0 bit EXTI_FTSR bis! \ falling edge for exti0 if needed
    0 bit EXTI_IMR bis!  \ enable exti0
    
    EXTI0_irq bit nvic_iser0 bis! \ enable exti0 in nvic
    CR ." Ready!"
;
    
\ setup code for multiline PB5 and PB8
: setup_PB5_PB8
    imode-float PB5 io-mode!
    imode-float PB8 io-mode!
\    imode-pull PB5 io-mode!
\    imode-pull PB8 io-mode!
    
    ['] gpio_isr irq-exti5 ! \ set isr for exti5-9 (!)
    
    $00E0 EXTICR2 bic! $0010 EXTICR2 bis!   \ PB5 for exti5
    $000E EXTICR3 bic! $0001 EXTICR3 bis!   \ PB8 for exti8
    5 bit 8 bit or EXTI_RTSR bis! \ rising edge for exti5 and exti8
    5 bit 8 bit or EXTI_IMR bis!  \ enable exti5 and exti8

    EXTI5_irq bit nvic_iser0 bis! \ enable exti5-9(!) in nvic
    CR ." Ready!"
;

