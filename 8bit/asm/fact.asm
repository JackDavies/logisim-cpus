$VAR COUNT
$VAR VAL
$VAR TEMP
$VAR MULTI
$VAR FACT 1
$VAR N 4
$VAR I 1

:FACT
SET TC 70
SET MP $FACT
LRA R0
SET MP $I
LRA R1
SET R3 #10
SET IP #C0

#10
SET TC 82
SET MP $FACT
WRA R0
SET MP $I
LRA AR
SET R0 1
ADD R0
WRA AR
SET MP $N
LRA R0
CMP R0
JGT #70
SET IP :FACT

#70
SET MP $FACT
LRA R0
SET TC 10
SET TC 10
SET TC 69

LRA AR
SET R1 48
ADD R1
SET MP $TEMP
WRA AR
LRA TC

:HALT
SET IP :HALT


:MULTRETURN
SET MP $TEMP
WRA R3
LRA IP

#C0
SET TC 77
SET AR 1
CMP R1
JET :MULTRETURN
SET R2 1
SET MP $COUNT
WRA R2
SET MP $VAL
WRA R0
SET MP $MULTI
WRA R1
:MULTLOOP
SET TC 76
SET MP $VAL
LRA AR
ADD R0
WRA AR
SET MP $COUNT
LRA AR
SET R2 1
ADD R2
WRA AR
CMP R1
JLT :MULTLOOP
SET MP $VAL
LRA R0
SET IP :MULTRETURN