# Forth_Imitation
Сафонова Ольга Данииловна, P3207, 368764

Вариант :
forth | stack | neum | **mc** ~~-> hw~~ | instr | **binary** ~~-> struct~~ | stream | port | pstr | prob2 | ~~cache~~ 

Без усложнения

# Forth
Используется стековая архитектура, поэтому все операции выполняются на стеке.

Используется постфиксная запись операций (например, 1 + 2 будет 1 2 +)

```<number>```    - запись числовой константы в стек

```"<line>"```    - запись строчной константы в стек; записывается посимвольно, последней записью идёт длина строки

```.```           - вывод и извлечение записи с вершины стека

```key```         - чтение с устройства и запись значение в стек

```drop```        - удаление вершины стека

```dup```         - дублирование числа на вершине стека и повторая запись

```swap```        - смена позиций двух чисел навершине стека 

```rot```         - смена двух чисел вокруг второго числа с вершины стека (например, 1 2 3 rot будет 3 2 1 на стеке)

---
```+```           - извлечение двух чисел со стека, сложение и запись результата в стек

```-```           - извлечение двух чисел со стека, вычитание вершины стека из второго числа и запись результата в стек

```and```         - извлечение двух чисел со стека, воспроизведение логического "и" и запись результата в стек

```or```          - извлечение двух чисел со стека, воспроизведение логического "или" и запись результата в стек

```<```           - извлечение двух чисел со стека, сравнение вершины и второго числа на стеке (вершина < число ?) и запись результата (0 или 1) в стек

---

```<commands> if <commands> else <commands> then``` - проверка на наличие ```true``` на вершине стека (не 0); в случае ```true``` воспроизведение блока после ```if```, иначе воспроизведение блока после ```else```, в конце переход к блоку ```then```

```<max value> <start value> do <commands> loop```          - ```do```: начало цикла, сохранение двух значений с вершины стека в буфферный стек; ```loop```: инкрементирование вершины буфферного стека и проверка, что вершина меньше второго значения; в случае, если меньше, воспроизведение блока после ```do```, иначе - окончание цикла;

```variable```     - получение адреса свободной ячейки в памяти

```!```            - сохранение второй записи со стека в адрес, хранящийся в вершине стека

```?```            - чтение записи по адресу с вершины стека и запись в стек

---

```: <procedure name> <code> ;``` - объявление процедуры

```<procedure name>```            - вызов процедуры

---

- Код выполняется последовательно с возможностью ветвления и циклов
- Комментарии не поддерживаются
- Присутствует поддержка числовых целочисленных литералов от 0 до 2^32-1

## Память
- Выделяется статически, при запуске модели
- Нет разделения памяти на коммандную и память данных
- Глобальная видимость данных
- Представлена в виде массива строк, размерностью 1000
- Cлово микрокоманды является 16-значным двоичным числом
- Адресация происходит по обращению по индексу массива
- Программа отображается на память, начиная с адреса 0
- Программисту доступны операции с главным стеком и памятью, начиная с адреса 100
- Числовые литералы хранятся по адресу, занятому при загрузке программы
- Строчные литералы хранятся по адресу 500 до тех пор, пока не будут загружены в стек
- При загрузке нового строчного литерала старый - затирается. Копия старого хранится в стеке
- Строки хранятся в паскаль-формате, массивом символов, где первой записью является длина строки. Хранение в строки в стеке подразумевает последним числом длину строки
- Запись в память и чтение из памяти осуществляются по адресу, хранящемуся на данный момент в вершине стека

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

Инструкции транслируются в мнемоники, после чего - в микрокоманды.

[Описание инструкций в мнемониках](decoder_data/forth_to_mnem.txt)

[Описание мнемоник в микропрограммах](decoder_data/mnemonic_description.txt)

- Числа занимают одну ячейку памяти
- Числа представлены в целочисленном виде от 0 до 2^32-1
- Строки представлены в виде массива символов, где начальная запись - длина строки; в стеке длина строки помещается в вершину стека
- Команды транслируются в микрокод
- Доступ к памяти осуществляется через адрес, хранящийся в вершине стека или в регистре pointer блока memory
- Есть возможность объявления процедуры через ```: <name> <code> ;```

Операции инструкций осуществляются над стеком: над вершиной, вершиной и вторым число, вершиной и третьим числом со стека.

Существует ветвление командами ```if <commands> else <commands> then```, где ```if``` проверяет наличие ненулевого числа на вершине стека, в случае ```true``` переходит на блок после ```if```, в случае ```false``` переходит на блок после ```else```, по завершению переходит на блок после ```then```.

Пример трансляции ```if``` в мнемоники:
```
reload main
reload null
do or
check zero
jump else
```

Работа иструкции ```jump```: выполняется, если осуществилась проверка ```check```, иначе происходит переход к следующей (будущей) мнемонике.  

Возможность создания цикла командами ```<max value> <min value> do <commands> loop```: если на вершине буфферного стека меньшее число, выполняется блок после ```do``` и при встрече ```loop``` - вершина стека инкрементируется; иначе, если на вершине буфферного стека большее либо равное число, выполняется блок после ```loop```.

[Микропрограммы мнемоник](decoder_data/mnemonic_description.txt)

Все микрокомманды:
```
1100000001001000 - snap stack pointer1     
1010000001001000 - snap stack1		        
1100000001010000 - snap stack pointer2	    
1010000001010000 - snap stack2		        
1001000001000000 - snap memory		        
1000100001001000 - snap tos1		        
1000100001010000 - snap tos2		        
1000010001000000 - snap alu		        
1000001001000000 - snap io input		    
1000001000100000 - snap io output		    
1000000101000000 - snap command pointer	
1000000011000000 - snap flags		        
1000000001000000 - snap null		        
1100000000101000 - reload stack pointer1	
1010000000101000 - reload stack1		    
1100000000110000 - reload stack pointer2	
1010000000110000 - reload stack2		    
1001000000100000 - reload memory		    
1001000000100100 - reload read memory      
1000100000101000 - reload tos1		        
1000100000110000 - reload tos2		        
1000000100100000 - reload command pointer	
0100000011000000 - add 			        
0010000011000000 - inc 			        
0001000011000000 - and 			        
0000100011000000 - or 			            
0000010001101100 - less			        
0000001011011000 - substract 			
0000000111011000 - dec 			        			        
```

**Микрокоманда строится по шаблону:**

Для управляющей (нулевой бит - 1) (позиция в списке - номер бита):

1. stack pointer
2. stack
3. memory
4. TOS
5. ALU
6. IO
7. 0
8. flags
9. for io: input; for other: snap
10. for io: output; for other: reload
11. buffer stack
12. main stack
13. special bit for reloading memory in only read format
14. 0
15. 0

1 | stack pointer | stack | memory | TOS | ALU | IO | 0 | flags | input/snap | output/reload | buffer | main | only read

Пример:<br>
11000000010010 <br>
1 | stack pointer | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | snap | 0 | 0 | main | 0 <br>
читается как: snap main stack pointer

Для операционной (нулевой бит - 0) (позиция в списке - номер бита):

1. add
2. increment
3. and
4. or
5. less
6. subtract
7. decrement
8. 0
9. zero flag
10. less flag
11. 0
12. 0
13. 0
14. 0
15. 0

0 | add | increment | and | or | less | substract | decrement | 0 | zero flag | less flag | 0 | 0 | 0

Пример: <br>
01000000110000 - add <br>
0 | add | 0 | 0 | 0 | 0 | 0 | 0 | 0 | zero flag | 0 | 0 | 0 | 0 <br>
читается как: add and touch n,z flags

Каждая инструкция транслируется в набор микрокоманд с помощью [листинга микропрограмм](decoder_data/microcommands.txt).

Количество тактов:
```
<number>    - 16
"<line>"    - 28
.           - 12
key         - 16
drop        - 10
dup         - 10
swap        - 39
rot         - 31
+           - 9
-           - 9
and         - 9
or          - 9
<           - 9

<values> 
if
<commands>
else 
<commands>
then        - 28 (min)

<max value>
<start value>
do
<commands>
loop        - 38 (min)

variable     - 18
!            - 9
?            - 13
```
[Пример программы](main_program_data/program.txt)

## Транслятор
Интерфейс командной строки: ```dotnet run .\program.txt .\input.txt .\output.txt .\forth_to_mnem.txt .\mnemonic_description.txt .\logging.log```

args:
0 - main programm
1 - file for input
2 - file for output
3 - forth to mnemonic file
4 - mnemonic to microcode file
5 - logging file

Реализован в файле [ControlUnit.cs](ControlUnit.cs) классом Decoder

Каждая инструкция транслируется в набор мнемоник с помощью [листинга мнемоник](decoder_data/forth_to_mnem.txt). Трансляция происходит по сопоставлению инструкционных слов и листинга. Условные переходы выполнены по принципу нахождения следующей значимой инструкции (например, при ```false``` после ```if``` будет искаться команда ```else```). Возврат при цикличности реализован с помощью стека возрата.

Процедуры, при встрече вызова, транслируются в набор инструкций

## Модель процессора
Входные данные:
- [описание трансляции](decoder_data/forth_to_mnem.txt)
- [описание мнемоник](decoder_data/mnemonic_description.txt)
- [программа](main_program_data/program.txt)
- [файл с входными данными](main_program_data/input.txt)

Выходные данные:
- [журнал операций и состояния процессора](main_program_data/logging.log)
- [файл вывода](main_program_data/output.txt)

Пояснение к журналу: пометка ```warning``` ставится на состояние, чтобы выделить его при форматировании. 

### Схемы

**DataPath**

- Реализован в классе [DataPath.cs](DataPath.cs)
- Сигнал ```snap``` защёлкивает значение в регистре
- Сигнал ```operation_type``` определяет тип операции, требуемой от ALU
- Сигнал ```jmp <type>``` определяет, до какой инструкции переходить
- Сигнал ```check <flag>``` проверяет наличие установленного флага
- Флаги: (zero, less)
- Наличие флага less означает, что на вершине стека находится меньшее значение, чем на второй позиции от вершины. 

![DataPath](https://sun9-64.userapi.com/impg/XtkyMjCEH2Iz1t9Y8N_ThYRqDuHzcmhVwnnEZg/xbUteTEfy9g.jpg?size=1175x1396&quality=96&sign=bd6d67a9199b65a96fa0a821c92a7eec&type=album)

Развёрнутая схема:

![DataPath](https://sun9-57.userapi.com/impg/V1a8OQC_YvaXiNE_F6P3gvxtMJth40LM5yeRgg/sAA8Dgjuxx4.jpg?size=936x1312&quality=96&sign=c6cfb51fec1e74d45c0e11f3790d64d8&type=album)

ControlUnit

- Реализован в классе [ControlUnit.cs](ControlUnit.cs)
- Обрабатывает флаги, полученные от DataPath

![ControlUnit](https://sun9-24.userapi.com/impg/te0RuRVRscuiMAtNOZHlUYrieawlghIPO2TLzg/UreWmVb-5pY.jpg?size=834x1042&quality=96&sign=56aca5b339679bb3f865843e2404a34f&type=album)

**Особенности модели**
- При встрече инструкции, она тут же выполняется
- Для журнала работы модели используется Serilog
- Количество инструкций на программу ограничено

## Тестирование
Алгоритмы тестов располагаются в директории tests

- [cat](/tests/golden/test_cat.txt)
- [hello user](/tests/golden/test_hello_user.txt)
- [fibonacci](/tests/golden/test_fibonacci_procedure.txt)
- [hello world](/tests/golden/test_hello_world.txt)

**Fibonacci**

[program.txt](main_program_data/program.txt)

```
: fib dup rot + ; 0 variable ! 1 1 10 0 do fib dup 1 and if drop else drop dup variable 1 - ? swap drop + variable ! drop drop then loop variable 1 - ? .
```
Programm translated to mnemonics as:
```
[12:04:44.654 [Information] 0
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] variable
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] !
reload main
to mem
[12:04:44.654 [Information] 1
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] 1
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] 10
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] 0
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] do
save address
inc sp2
tos2 to 2nd
inc sp2
stack1 to stack2
tos1 to tos2
2nd to tos1
dec sp1
2nd to tos1
dec sp1
reload buffer
do less
check not less
jump loop
[12:04:44.654 [Information] dup
inc sp1
tos1 to 2nd
[12:04:44.654 [Information] rot
inc sp2
tos2 to 2nd
inc sp2
dec sp1
stack1 to stack2
to tos2
tos1 to 2nd
tos2 to tos1
inc sp1
dec sp2
2nd to tos2
dec sp2
[12:04:44.654 [Information] +
reload main
do add
to tos1
dec sp1
[12:04:44.654 [Information] dup
inc sp1
tos1 to 2nd
[12:04:44.654 [Information] 1
inc sp1
tos1 to 2nd
mem to tos1
[12:04:44.654 [Information] and
reload main
do and
to tos1
dec sp1
[12:04:44.654 [Information] if
reload main
reload null
do or
check zero
jump else

...
```

Console output: ``` Tick count: 2517
Microcommands count: 1801 | Program size in bit: 28816 | Instruction count: 161 ```

Output: ```188```.

Logging: [logging.log](main_program_data/logging.log)

## End
```
| ФИО                       | prog         | line count | prog size | microinstr count | instr count | tick count | variant

| Сафонова Ольга Данииловна | hello user   | 1510       | 1486      | 4401             | 336         | 5496       | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |

| Сафонова Ольга Данииловна | cat          | 34387      | 162      | 87533            | 9375        | 112547     | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |

| Сафонова Ольга Данииловна | fibonacci    | 646        | 782      | 1801             | 161         | 2517       | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |

| Сафонова Ольга Данииловна | hello world  | 124        | 144      | 417              | 29          | 516        | forth | stack | neum | mc | instr | binary | stream | port | pstr | prob2 |
```