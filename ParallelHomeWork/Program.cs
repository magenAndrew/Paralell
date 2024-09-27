
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

var arrayInt = new long[] { 100000, 1000000, 10000000 };
var arrayInt1 = new long[arrayInt[0]];
var arrayInt2 = new long[arrayInt[1]];
var arrayInt3 = new long[arrayInt[2]];
var result = new long[3, 3, 2];
//инициализация переменных
for (var i = 0; i < arrayInt3.Length; i++) {
    if (i < arrayInt1.Length)
        arrayInt1[i] = i;
    if (i < arrayInt2.Length)
        arrayInt2[i] = i; 
    arrayInt3[i] = i;
}
//вычисления
//1.Напишите вычисление суммы элементов массива интов:
//2. Замерьте время выполнения для 100 000, 1 000 000 и 10 000 000
//3. Укажите в таблице результаты замеров, указав:
//Окружение(характеристики компьютера и ОС)
Console.WriteLine($"--------------------------------------------------------------------------");
Console.WriteLine("Версия Windows: {0}",    Environment.OSVersion);
Console.WriteLine("64 Bit операционная система? : {0}",   Environment.Is64BitOperatingSystem ? "Да" : "Нет");
Console.WriteLine("Имя компьютера : {0}",  Environment.MachineName);
Console.WriteLine("Число процессоров : {0}",   Environment.ProcessorCount);
Console.WriteLine($"--------------------------------------------------------------------------");
Console.WriteLine($"| Суммирование     |  Эементов   |        Сумма         |    время       |");
Console.WriteLine($"--------------------------------------------------------------------------");

//Обычное
CalcSummOriginal(arrayInt1, 0);
CalcSummOriginal(arrayInt2, 1);
CalcSummOriginal(arrayInt3, 2);
Console.WriteLine($"--------------------------------------------------------------------------");

//Параллельное(для реализации использовать Thread, например List) ???
CalcSummTread(arrayInt1, 0);
CalcSummTread(arrayInt2, 1);
CalcSummTread(arrayInt3, 2);
Console.WriteLine($"--------------------------------------------------------------------------");

//Параллельное с помощью LINQ
CalcSummParallelLinq(arrayInt1, 0);
CalcSummParallelLinq(arrayInt2, 1);
CalcSummParallelLinq(arrayInt3, 2);


    void CalcSummOriginal(long[] arrayInt,int pos) {
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    long sum = 0;
    for (var i = 0; i < arrayInt.Length; i++)
    {
        sum += arrayInt[i];
    }
    stopWatch.Stop();
    Console.WriteLine($" Обычное           | {arrayInt.Length,10} | {sum,20}  | {stopWatch.ElapsedMilliseconds}");
}

void CalcSummTread(long[] arrayInt, int pos)
{
    var lockObject = new object();
    var stopWatch = new Stopwatch();
    var maxNumber = arrayInt.Length;
    var threadsCount = 2;
    stopWatch.Start();
    long sum = 0;
    var threads = new Thread[threadsCount];
    WaitHandle[] waitHandles = new WaitHandle[threadsCount];
    var output = new long[threadsCount];
    for (var i = 0; i < threadsCount; i++)
    {
        var index = i;
        var start = (i * maxNumber) / threadsCount;
        var end = ((i + 1) * maxNumber) / threadsCount;
        var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
        var thread = new Thread
        (
            () =>
            {
                long result = 0;
                for (var i = start; i < end; i++)
                {
                    result += arrayInt[i];
                }
                lock (lockObject) { 
                    sum += result; 
                };
                handle.Set();
            });
        waitHandles[i] = handle;

        thread.Start();
    };
    WaitHandle.WaitAll(waitHandles);
    stopWatch.Stop();
    Console.WriteLine($" Thread (потоков {threadsCount})| {arrayInt.Length,10} | {sum,20}  | {stopWatch.ElapsedMilliseconds}");
}

void CalcSummParallelLinq(long[] arrayInt, int pos)
{
    var lockObject = new object();
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    long sum  = arrayInt.AsParallel().Sum();
    stopWatch.Stop();
    Console.WriteLine($" Параллельное LINQ | {arrayInt.Length,10} | {sum,20}  | {stopWatch.ElapsedMilliseconds}");
}
