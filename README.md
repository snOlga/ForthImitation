# Forth_Imitation
Сафонова Ольга Данииловна, P3207, 368764

Вариант :
forth | stack | neum | **mc** -> hw | instr | **binary** -> struct | stream | port | pstr | prob2 | ~~cache~~ 

Без усложнения

# Fort
Используется стековая архитектура, поэтому все операции выполняются с учётом стека.

```<number>```    - push contsnt to stack

```"<line>"```    - push string to stack as a char array, first node is string length

```.```           - print and pop top of the stack

```key```         - read and push a value from input

```drop```        - pop top of the stack

```dup```         - duplicate top of the stack and push it

```swap```        - change position of two values on top of the stack

```rot```         - change position of two values around second value from top of the stack (ex. 1 2 3 rot - will be 3 2 1)

---
```+```           - add top of the stack with second number from top

```-```           - subtract top of the stack from second number from top

```and```         - logic 'and' with two numbers from stack

```or```          - logic 'or' with two numbers from stack

```<```           - check if top of the stack is less than second value in stack 

---

```<values>``` <br>
```if``` <br>
```<commands>```<br>
```else ```<br>
```<commands>```<br>
```then```        - check if there is 'true' (positive number) on top of the stack; jump to else if negative or do program

```<max value>```<br>
```<start value>```<br>
```do```          - start looping and do it according to 'if' principle; saves looping values in buffer stack
```<commands>```<br>
```loop```        - increment top of the buffer stack and jump to next command after 'do' or end looping; working according to 'if' principle     

```variable```     - returns the index of free space of memory

```!```            - push data using the index in the top of the stack to memory

```?```            - read data and push it to stack using the index in the top of the stack from memory

---

```: <procedure name> <code> ;``` - procedure declaration
```<procedure name>```            - procedure call

---

- Код выполняется последовательно с возможностью ветвления и циклов
- Комментарии не поддерживаются
- Присутствует поддержка числовых целочисленных литералов от -2^32 до 2^32-1

## Память
- Выделяется статически, при запуске модели
- Нет разделения памяти на коммандную и память данных
- Глобальная видимость данных
- Представлена в виде массива строк, размерностью 1000
- Машинное слово не определено
- Адресация происходит по обращению по индексу массива
- Программа отображается на память, начиная с 0 адреса
- Программисту доступны операции с главным стеком и памятью, начиная с адреса 100
- Числовые литералы хранятся по адресу, занятому при загрузке программы
- Строчные литералы хранятся по адресу 500 до тех пор, пока не будут загружены в стек
- При загрузке нового строчного литерала старый - затирается. Копия старого хранится в стеке
- Строки хранятся в паскаль-формате, массивом символов, где первой записью является длина строки. Хранение в строки в стеке подразумевает последним числом длину строки
- Запись в память и чтение из памяти осуществляются по адресу, хранящемуся на тот момент в вершине стека.

```
---------------------
| 0     programm    |
| n     and         |
| n+m   numeric     |
| ...   constants   |
---------------------
| 100               |
| ...   variables   |
---------------------
| 500   string      |
| ...   constants   |
---------------------
```

## Система команд
- Отдельно хранятся числа и строки
- Числа занимают одну ячейку памяти
- Числа представлены в целочисленном виде от -2^32 до 2^32-1
- Строки представлены в виде массива символов, где начальная запись - длина строки
- Команды транслируются в микрокод
- Доступ к памяти осуществляется через адрес, хранящийся в вершине стека или в регистре pointer блока memory
- Есть возможность объявления процедуры через ```:``` в начале строки

Операции инструкций осуществляются над стеком: над вершиной, вершиной и вторым число, вершиной и третьим числом со стека.

Существует ветвление командами ```if <commands> else <commands> then```, где ```if``` проверяет наличие положительного числа на вершине стека и в случае ```false``` переходит на блок else, иначе по завершению переходит на блок ```then```.

Возможность создания цикла командами ```<max value> <min value> do <commands> loop```, которые работают по принципу ```if```: если на вершине стека меньшее число, выполняется блок после ```do``` и при встрече ```loop``` - вершина стека инкрементируется; иначе, если на вершине стека большее число, выполняется блок после ```loop```.

[Микропрограммы команд, расписанные мнемониками](microcode_mnemonic.txt)

Все микрокомманды:
```
snap stack pointer1     11000000010010
snap stack1		        10100000010010
snap stack pointer2	    11000000010100
snap stack2		        10100000010100
snap memory		        10010000010000
snap tos1		        10001000010010
snap tos2		        10001000010100
snap alu		        10000100010000
snap io input		    10000010010000
snap io output		    10000010001000
snap command pointer	10000001010000
snap flags		        10000000110000
snap null		        10000000010000
reload stack pointer1	11000000001010
reload stack1		    10100000001010
reload stack pointer2	11000000001100
reload stack2		    10100000001100
reload memory		    10010000001000
reload read memory      10010000001001
reload tos1		        10001000001010
reload tos2		        10001000001100
reload command pointer	10000001001000
add 			        01000000110000
inc 			        00100000110000
and 			        00010000110000
or 			            00001000110000
less			        00000100011011
subtract 			    00000010110110
dec 			        00000001110110
```

**Микрокоманда строится по шаблону:**

Для управляющей (нулевой бит - 1) (позиция в списке - номер бита):

1. stack pointer
2. stack
3. memory
4. TOS
5. ALU
6. IO
7. ~~deleted~~ 0
8. flags
9. for io: input; for other: snap
10. for io: output; for other: reload
11. main stack
12. buffer stack
13. special bit for reloading memory in only read format

Для операционной (нулевой бит - 0) (позиция в списке - номер бита):

1. add
2. incriment
3. and
4. or
5. less
6. subtract
7. decrement
8. negative flag
9. zero flag
10. less flag
11. 0
12. 0
13. 0

Каждая инструкция транслируется в набор микрокоманд с помощью [листинга микропрограмм](microcode.txt).

Количество тактов:
```
<number>    - 9
"<line>"    - 9
.           - 10
key         - 9
drop        - 8
dup         - 8
swap        - 44
rot         - 41
+           - 11
-           - 11
and         - 11
or          - 11
<           - 11

<values> 
if          - 5
<commands>
else 
<commands>
then

<max value>
<start value>
do          - 16
<commands>
loop        - 11

variable     - 9
!            - 4
?            - 9
```
[Пример программы](program.txt)

## Транслятор
Интерфейс командной строки: ``` dotnet run <main programm>.txt <file for input>.txt <file for output>.txt <main microcode>.txt <logging file>.log/.txt```

! должен быть указан только относительный путь файлов, лежащих в той же директории, что и программа !

args:
0 - main programm
1 - file for input
2 - file for output
3 - main microcode
4 - logging file

Реализован в файле [ControlUnit.cs](ControlUnit.cs) классом Decoder


Каждая инструкция транслируется в набор микрокоманд с помощью [листинга микропрограмм](microcode.txt). Трансляция происходит по сопоставлению инструкционных слов и листинга. Условные переходы выполнены по принципу нахождения следующей значимой инструкции (например, при ```false``` после ```if``` будет искаться команда ```else```).

На строке может быть размещена либо команда, либо объявление процедуры.

## Модель процессора
Входные данные:
- [машинный код](microcode.txt)
- [программа](program.txt)
- [файл с входными данными](input.txt)

Выходные данные:
- [журнал микроопераций и состояния процессора](logging.log)
- [вывод процессора](output.txt)

Пояснение к журналу: пометка ```warning``` ставится на состояние, чтобы выделить его при форматировании. 

### Схемы

**DataPath**

- Реализован в классе [DataPath.cs](DataPath.cs)
- Сигнал ```snap``` защёлкивает значение в регистре
- Сигнал ```operation_type``` определяет тип операции, требуемой от ALU
- Флаги: (negative, zero, less)
- Наличие флага less означает, что на вершине стека находится меньшее значение, чем на второй позиции от вершины. 

![DataPath](https://sun9-16.userapi.com/impg/k2Xg1xtd-lv66p5ZPp3vYNUSv02eHZLDvH3CBw/55jtOJczWR4.jpg?size=1111x1514&quality=96&sign=a0e748f7a004dca91963004531ca3bcf&type=album)

Только с сигнальными линиями:

![DataPathSignal](https://sun9-24.userapi.com/impg/yGr48wvkl8Zd-SQi1aWQQRFaEb6evw_XKB9u4w/Q8nltW2axjQ.jpg?size=930x1475&quality=96&sign=3065cf0b4e7a9ca15bbd7a7e6ff81d8e&type=album)



ControlUnit

- Реализован в классе [ControlUnit.cs](ControlUnit.cs)
- Обрабатывает флаги, полученные от DataPath

![ControlUnit](https://sun9-45.userapi.com/impg/19-gKYKJB5Egd5vg9YJz3EM1BhF9ckve29xYAg/oPZuYltJqXc.jpg?size=848x836&quality=96&sign=298467a74afa461afb71d31fea70f85a&type=album)

**Особенности модели**
- При встрече инструкции, она тут же выполняется
- Для журнала работы модели используется Serilog
- Количество инструкций на программу ограничено
- Ошибки (не технические) при работе игнорируются, но записываются в журнал.

## Тестирование
Алгоритмы тестов располагаются в директории tests

- [cat](/tests/test_cat.txt)
- [hello user](/tests/test_hello_user.txt)
- [fibonacci](/tests/test_fibonacci_procedure.txt)

**Fibonacci**

[program.txt](program.txt)

```
: fib dup rot + ;
1
1
10
0
do
dup
.
" "
drop
.
fib
loop
```
Translated to memory:
```
1
1
10
0
do
dup
.
" "
drop
.
dup
rot
+
loop
```
Translated to microcode:
```
"11000000001010" - 1
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10010000001000"
"10001000010010"

"11000000001010" - 1
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10010000001000"
"10001000010010"

"11000000001010" - 10
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10010000001000"
"10001000010010"

"11000000001010" - 0
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10010000001000"
"10001000010010"

"11000000001100" - do
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010100"
"10100000001010"
"10100000010100"
"10001000001010"
"10001000010100"
"11000000001010"
"10100000001010"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"
"11000000001010"
"10100000001010"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"

"11000000001010" - dup
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10001000010010"

"10001000001010" - .
"10000010001000"
"11000000001010"
"10100000001010"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"

"11000000001010" - " "
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10010000001000"
"10001000010010"

"11000000001010" - drop
"10100000001010"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"

"10001000001010" - .
"10000010001000"
"11000000001010"
"10100000001010"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"

"11000000001010" - dup
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010010"
"10001000001010"
"10100000010010"
"10001000010010"

"11000000001100" - rot
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010100"
"10001000001100"
"10100000010100"
"11000000001100"
"10000000010000"
"10000100010000"
"00100000110000"
"11000000010100"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"
"10100000001010"
"10100000010100"
"10001000010100"
"10001000001010"
"10100000010010"
"10001000001100"
"10001000010010"
"11000000001010"

"10001000001010" - +
"10100000001010"
"10000100010000"
"01000000110000"
"10000000110000"
"10001000010010"
"11000000001010"
"10000000010000"
"10000100010000"
"00000001110110"
"11000000010010"

"10001000001100" - loop
"10000000010000"
"10000100010000"
"00100000110000"
"10001000010100"
"10001000001100"
"11000000001100"
"10100000001100"
"10001000001100"
"10000100010000"
"00000100011011"
"10000000110000"
```

Console output: ``` Tick count: 2507 ```.

Output: ```1 2 3 5 8 13 21 34 55 89 ```.

Logging: [logging.log](logging.log)

## End
```
| Сафонова Ольга Данииловна | Hello User | 25 | 13384 bit | 956 | 70 | 1833 | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |

| Сафонова Ольга Данииловна | cat | 6 | 1199954 bit | 85711 | 8292 | 196271 | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |

| Сафонова Ольга Данииловна | fibonacci | 13 | 18592 bit | 1328 | 95 | 2507 | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |
```