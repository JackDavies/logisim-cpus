SET R0 1
SET R1 10
SET AR 1
:LOOP
ADD R0
CMP R1
JLT :LOOP
SET AR 1
SET IP :LOOP