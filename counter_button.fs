\ this example blinks pc13 after 6(!) raising edges on PA0


$40000000 constant TIM2  
       TIM2 $0 + constant TIM2_CR1
       TIM2 $4 + constant TIM2_CR2
       TIM2 $8 + constant TIM2_SMCR
       TIM2 $C + constant TIM2_DIER
       TIM2 $10 + constant TIM2_SR
       TIM2 $14 + constant TIM2_EGR
       TIM2 $18 + constant TIM2_CCMR1_Output
       TIM2 $18 + constant TIM2_CCMR1_Input
       TIM2 $1C + constant TIM2_CCMR2_Output
       TIM2 $1C + constant TIM2_CCMR2_Input
       TIM2 $20 + constant TIM2_CCER
       TIM2 $24 + constant TIM2_CNT
       TIM2 $28 + constant TIM2_PSC
       TIM2 $2C + constant TIM2_ARR
       TIM2 $34 + constant TIM2_CCR1
       TIM2 $38 + constant TIM2_CCR2
       TIM2 $3C + constant TIM2_CCR3
       TIM2 $40 + constant TIM2_CCR4
       TIM2 $48 + constant TIM2_DCR
       TIM2 $4C + constant TIM2_DMAR

$40010000 constant AFIO  
       AFIO $0 + constant AFIO_EVCR
       AFIO $4 + constant AFIO_MAPR
       AFIO $8 + constant AFIO_EXTICR1
       AFIO $C + constant AFIO_EXTICR2
       AFIO $10 + constant AFIO_EXTICR3
       AFIO $14 + constant AFIO_EXTICR4
       AFIO $1C + constant AFIO_MAPR2
 
omode-od pc13 io-mode!
 
\ 11 constant tim1en
\ tim1en bit RCC-APB2ENR bis! \ enable timer1

\ afio is already enabled
\ 0 constant afioen
\ afioen bit RCC-APB2ENR bis! \ enable afio

\ no afio remapping, we use PA0 for tim2 ch1

imode-float pa0 io-mode!
 
 
0 constant tim2en
tim2en bit RCC-APB1ENR bis! \ enable timer2

0 constant cen
cen bit tim2_cr1 bic! \ disable counting

0 tim2_PSC h! \ prescaler
5 tim2_arr h! \ count max

\ external Clock mode 1
%111 tim2_smcr bis!
\ Trigger External input (raising edge since bit 15 is zero)
%1110000 tim2_smcr bis!
 

\ interupt service routine
0 constant uif
: t2isr
    pc13 iox! \ onoff LED
    
    uif bit tim2_sr bic! \ reset irq bit
    eint
;

['] t2isr irq-tim2 !

28 constant tim2irq
tim2irq bit nvic_iser0 bis!

0 constant uie
uie bit TIM2_DIER bis! \ enable update interupt on tim2
    
cen bit tim2_cr1 bis! \ start counting

