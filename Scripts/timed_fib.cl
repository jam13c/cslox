fun fib(n)
{
	if(n <= 1) return n;
	return fib(n-2) + fib(n-1);
}

for(var i = 0;i<20;i=i+1)
{
   var start = clock();
   var result = fib(i);
   var time = clock() - start;
   print result + "(in "+ time/1000 + "s)";
}