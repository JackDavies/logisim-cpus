#include <stdio.h>

int main(){
	int lineCounter = 0;
	for (int i = 1; i <= 255; i++){
		if (lineCounter < 7){
			printf("%i ", i );
		}else{
			printf("%i\n", i);
			lineCounter = 0;
		}
		lineCounter++;
	}
}
