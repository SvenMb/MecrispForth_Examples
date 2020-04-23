 \ this example blinks pc13 with tim2 in 0.5s


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
        
omode-od pc13 io-mode!
 
\ 11 constant tim1en
\ tim1en bit RCC-APB2ENR bis! \ enable timer1

0 constant tim2en
tim2en bit RCC-APB1ENR bis! \ enable timer2

0 constant cen
cen bit tim2_cr1 bic! \ disable counting

575 tim2_PSC h! \ prescaler
62499 tim2_arr h! \ count max
\ 10sek with 72MHz

0 constant uif
: t2isr
    pc13 iox!
    
    uif bit tim2_sr bic! \ reset irq bit
    eint
;

['] t2isr irq-tim2 !

\ 25 constant tim1_up
\ tim1_up bit nvic_iser0 bis!

28 constant tim2
tim2 bit nvic_iser0 bis!

0 constant uie
\ 6 constant tie

\ tie bit TIM1_DIER bis!
uie bit TIM2_DIER bis! \ enable update interupt
    
cen bit tim2_cr1 bis! \ start counting

