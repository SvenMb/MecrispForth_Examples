\ needs rtc.fs from jcw
\ needs time.fs 


: rtcisr
    1 rtc-crl bit@ not if \ check if new second
	exit
    then
    1 rtc-crl bic! \ reset second flag

    now CR time time.    
    \ now CR . \ or just print timestamp if you don't like my time.fs
;

: rtc_isr_init
    rtc-init
    500 ms	
    now time time.
    \ now CR . \ or just print timestamp if you don't like my time.fs

    ['] rtcisr irq-rtc ! \ write my isr to irq vector tab
    3 bit nvic_iser0 bis! \ enable irq vector
    500 ms
    0 bit rtc bis!        \ enable second irq in rtc-crh
;
