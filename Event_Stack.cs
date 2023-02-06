
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame.TPlayer;

namespace GameLogica {
    public partial class TGame {
        /// <summary>
        ///     содержит в себе необходимые поля и методы для реализации полноценного стека всех событий на карте Travian.
        /// </summary>
        /// <remarks>
        ///     Стек хранит следующие события: <br/>
        ///     - Передвижения войск по карте. <br/>
        ///     - Запущенные постройки. <br/>
        ///     - Отправка/Принятие ресурсов через рынок. <br/>
        ///     - Строительства юнитов в казарме, конюшне, мастерской, дворце, резиденции. <br/>
        ///     - Обучения новых войск в академии. <br/>
        ///     - Прокачки войск в кузнице. <br/>
        /// </remarks>
        public class TEvent_Stack {
            //public TEvent_Stack(WindowsInterface.Form1.TLANGUAGES Languages) { this.Languages = Languages; }
            public WindowsInterface.Form1.TLANGUAGES Languages;

            /// <summary>
            ///     Поле <b> Stack </b> хранит все возможные события в каждой строке. <br/>
            ///     <b> TMultiEvent </b> - общий подкласс для всех возможных типов событий. <br/>
            ///     В каждой строке описан тип события, владелец события, координаты, войска, пойстройки и т.д.
            /// </summary>
            public LIST<TMultiEvent> Stack = new LIST<TMultiEvent>();
            /// <summary> Перегрузка <b>[]</b> для листа <b>LIST Stack[];</b> </summary>
            public class LIST<T> : List<TMultiEvent> {
                public new TMultiEvent this[int i] {
                    get { var result = i < Count ? base[i] : new TMultiEvent();//заглушка при выходе за диапазон
                        //if (i < 0 || i >= Count) MessageBox.Show($"Error. Индекс вне диапазона.\nindex = {i}\tEvent_Stack.Stack[].Count = {Count}\n" +
                        //    $"Ошибка может возникнуть при попытке прочитать элемент стека событий Stack[index] после его удаления управляющим потоком.");
                        return result;
                    } set { if (i < Count) base[i] = value;//при выходе за диапазон, присвоение просто не произойдёт
                        //if (i < 0 || i >= Count) MessageBox.Show($"Error. Индекс вне диапазона.\nindex = {i}\tEvent_Stack.Stack[].Count = {Count}\n" +
                        //    $"Ошибка может возникнуть при попытке присвоить элементу стека событий Stack[index] после его удаления управляющим потоком.");
                    }
                }
            }

            /// <summary> Метод осуществляет сортировку стека. </summary>
            /// <remarks> В сортировке нет нужды, если нет эффекта наблюдателя. т.е. если игрок не смотрит на панель передвижения войск. </remarks>
            private void Sorting_List() {
                //сортировка списков осуществляется по полю TMultiEvent.timer
                if (Stack.Count > 1) Stack.Sort(/*delegate*/(TMultiEvent x, TMultiEvent y)=>{ return x.timer.CompareTo(y.timer); });
            }

            /// <summary> Метод вычисляет есть ли передвижения войск входящие/исходящие в переданных координатах. </summary>
            /// <value> <b> <paramref name="Коры"/>: </b> проверяемые координаты формата <b> Cell[Coordinates_World_Travian.Х][Coordinates_World_Travian.Y] </b> </value>
            /// <returns> Возвращает <b> true </b> если передвижения есть и <b> false</b> - если нет. </returns>
            public bool IsMove(Point Коры) {
                bool move = false;//default
                for (int i = 0; i < Stack.Count; i++) {
                    if (Stack[i].TypeEvent >= Type_Event.ATTACK && Stack[i].TypeEvent <= Type_Event.ESTABLISH_A_SETTLEMENT)
                        if (Stack[i].Cell_Start == Коры || Stack[i].Cell_Finish == Коры) { move = true; break; }
                }
                return move;
            }

            /// <summary> Метод вычисляет запущено ли строительство в переданных координатах. </summary>
            /// <value> <b> <paramref name="Coordinates"/>: </b> проверяемые координаты формата <b> Cell[Coordinates_World_Travian.Х][Coordinates_World_Travian.Y] </b> </value>
            /// <returns> Возвращает <b> true </b> если в ведённых координатах ведётся строительство и <b> false</b> - если нет. </returns>
            public bool IsConstruction(Point Coordinates) {
                bool move = false;//default
                for (int i = 0; i < Stack.Count; i++) {
                    if (Stack[i].TypeEvent == Type_Event.Construction)
                        if (Stack[i].Cell_Start == Coordinates || Stack[i].Cell_Finish == Coordinates) { move = true; break; }
                }
                return move;
            }

            /// <summary> Метод вычисляет максимальный таймер в очереди строительства. <br/> Нужно чтобы последующие постройки прибавляли своё время к нему. Чтобы была очередь, а не параллельное строительство. </summary>
            /// <value>
            ///     <b><paramref name="Coordinates"/>:</b> проверяемые координаты формата <b> Cell[Coordinates_World_Travian.Х][Coordinates_World_Travian.Y] </b> <br/>
            ///     <b><paramref name="Type"/>:</b> тип проверяемого события. <br/>
            /// </value>
            /// <returns> Возвращает максимальный таймер в очереди строительства и <br/> 0 если постройки не запущены. </returns>
            public int MaxTime(Point Coordinates, Type_Event Type) {
                int max = 0;
                for (int i = 0; i < Stack.Count; i++) {
                    if (Stack[i].TypeEvent == Type) if (Coordinates == Stack[i].Cell_Start)
                            if (Stack[i].timer > max) max = Stack[i].timer;
                } return max;
            }

            /// <summary> Метод считает в стеке событий количество добавленных построек в очередь строительства в переданном слоте включая снос здания. </summary>
            /// <value>
            ///     <b><paramref name="Coordinates"/>:</b> проверяемые координаты в системе координат <b> Map.Cell[x][y] </b> <br/>
            ///     <b><paramref name="NumberSlot"/>:</b> номер слота. <br/>
            /// </value>
            /// <returns> Возвращает <b>true</b> если в очереди строительства больше 1 постройки в слоте <b>NumberSlot</b>, в противном случае вернёт <b>false.</b> </returns>
            public bool IsMoreThanOneConstructionInTheSlot(Point Coordinates, int NumberSlot) { int count = 0;
                for (int i = 0; i < Stack.Count; i++)
                    if (Stack[i].TypeEvent == Type_Event.Construction) if (Coordinates == Stack[i].Cell_Start)
                            if (NumberSlot == Stack[i].Slot) { count++; if (count > 1) return true; }
                return false;
            }
            /// <summary> Метод считает в стеке событий количество всех добавленных построек в очередь строительства включая снос здания. </summary>
            /// <value> <b><paramref name="Coordinates"/>:</b> проверяемые координаты в системе координат <b> Map.Cell[x][y] </b> </value>
            /// <returns> Возвращает количество построек в очереди в слоте <b>NumberSlot</b>, если очередь пуста, вернёт 0. </returns>
            public int Constructions_Count(Point Coordinates) { int count = 0;
                for (int i = 0; i < Stack.Count; i++)
                    if (Stack[i].TypeEvent == Type_Event.Construction) if (Coordinates == Stack[i].Cell_Start) count++;
                return count;
            }

            /// <summary> Метод добавляет строки в стек событий. <br/> Добавляются и передвижения войск и постройки. </summary>
            /// <remarks>
            ///     Пример кода для добавления строк с войсками: <br/>
            ///     <b> Player.Event_Stack.Add(60, RED_attack, None, Folk.Галлы, p1, p2, new int[] { 100, 0, 500, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО); </b> <br/>
            ///     <b> Player.Event_Stack.Add(90, GREEN_shield, Возвращение_войск, Folk.Галлы, p1, p2, new int[] { 100, 0, 500, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 100, 100, 100, 100, 50 }, -1, -1, Buildings.ПУСТО); </b> <br/>
            ///     Пример кода для добавления строк с постройками: <br/>
            ///     Point p = (присваиваем координаты относительно массива) <br/>
            ///     <b> Player.Event_Stack.Add(60, Construction, None, Folk.Галлы, p, p, null, null, 6, 1, Buildings.Ферма); </b> <br/>
            ///     <b> Player.Event_Stack.Add(60, Construction, None, Folk.Галлы, p, p, null, null, 27, 2, Buildings.Главное_Здание); </b> <br/>
            /// </remarks>
            /// <value> 
            ///     <b> <paramref name="Timer"/>: </b> время в секундах которое должно пройти до срабатывания события. <br/>
            ///     <b> <paramref name="TypeEvent"/>: </b> тип событий. <br/>
            ///     <b> <paramref name="TypeMovement"/>: </b> тип передвижений войск. <br/>
            ///     <b> <paramref name="Nation"/>: </b> название нации которая сгенерировала событие. <br/>
            ///     <b> <paramref name="Cell_Start"/>: </b> координаты ячейки в которой началось событие в системе координат <b>Map.Cell[x][y]</b>. <br/>
            ///     <b> <paramref name="Cell_Finish"/>: </b> координаты ячейки в которой событие закончилось в системе координат <b>Map.Cell[x][y]</b>. <br/>
            ///     <b> <paramref name="Units"/>: </b> Массив из 11 элементов. Кол-во юнитов каждого типа перемещаемых войск. [0..9] - войска, [10] - герой. <br/>
            ///         (если тип события = строительство, тогда int[] Units = null). <br/>
            ///         Массив нужен для проведения симуляции боя и генерации отчёта. <br/> 
            ///     <b> <paramref name="Farm"/>: </b> Массив из 5 элементов. Кол-во ресурсов каждого типа: wood, clay, iron, crop, gold. <br/>
            ///         (если тип события = строительство, тогда int[] Farm = null). <br/>
            ///         Массив нужен для генерации отчёта и добавления нафармленных ресурсов в хранилища. <br/> 
            ///     <b> <paramref name="Slot"/>: </b> номер слота в котором ведётся строительство. <br/>
            ///     <b> <paramref name="lvl"/>: </b> уровень строящейся постройки. <br/>
            ///     <b> <paramref name="id"/>: </b> имя (название) строящейся постройки. <br/>
            ///     <b> <paramref name="Max_Count_Constructions"/>: </b> поле <b> Player.Max_Count_Constructions; </b> - максимальное количество одновременных построек выбранной нацией (всё что больше, просто не добавится). <br/>
            /// </value>
            /// <returns> Возвращает <b> true </b> если добавление прошло успешно. </returns>
            public bool Add(int Timer, Type_Event TypeEvent, Type_Movement TypeMovement, Folk Nation, Point Cell_Start, Point Cell_Finish,
                            int[] Units, int[] Farm, int Slot, int lvl, Buildings id, bool Destruction) {
                //на внешнем контуре сюда отправляю null если это не передвижения войск, но массивы всё равно создаются.
                if (Units == null) Units = new int[11]; if (Farm == null) Farm = new int[5];
                Stack.Add(new TMultiEvent(Timer, TypeEvent, TypeMovement, Nation, Cell_Start, Cell_Finish, Units, Farm, Slot, lvl, id, Destruction));
                //Sorting_List();//сортировка оказалось не нужна
                return true;
            }

            /// <summary> 
            ///     Метод сохраняет поля класса <b>TEvent_Stack</b> в файл: <b>Event_Stack.DAT</b> <br/>
            ///     Сохраняется каждая строка листа <b>TMultiEvent.List;</b> в цикле. <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            ///     Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b>
            /// </summary>
            public void SaveStack(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Event_Stack.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(Stack.Count);//сохраняем количество строк в стеке первым символом
                        if (Stack.Count <= 0) return;
                        for (int i = 0; i < Stack.Count; i++) {
                            bw.Write(Stack[i].timer);              bw.Write((int)Stack[i].TypeEvent);
                            bw.Write((int)Stack[i].TypeMovement);  bw.Write((int)Stack[i].Nation);
                            bw.Write(Stack[i].Cell_Start.X);       bw.Write(Stack[i].Cell_Start.Y);
                            bw.Write(Stack[i].Cell_Finish.X);      bw.Write(Stack[i].Cell_Finish.Y);
                            bw.Write(Stack[i].Slot);       bw.Write(Stack[i].lvl);
                            bw.Write((int)Stack[i].ID);    bw.Write(Stack[i].Destruction);
                            //массивы сохранять последними
                            for (int n = 0; n < Stack[i].Units.Length; n++) {
                                bw.Write(Stack[i].Units[n]);/*11*/
                                if (n < Stack[i].Farm.Length) bw.Write(Stack[i].Farm[n]);/*5*/
                            }

                            LoadProcess.LoadText.Text = Text + " Stack[" + i + "]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary>
            ///     Метод загружает в стек событий данные из бинарного файла <b> Event_Stack.DAT </b> <br/>
            ///     Все значения бинарного файла заполняют соответствующие поля <b> TMultiEvent </b>. <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void LoadStack(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Event_Stack.DAT", FileMode.Open)) {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                        int Stack_Count = br.ReadInt32();/*0*/ if (Stack_Count <= 0) return;
                        for (int i = 0; i < Stack_Count; i++) {
                            var MultiEvent = new TMultiEvent {
                                timer = br.ReadInt32(), TypeEvent = (Type_Event)br.ReadInt32(),
                                TypeMovement = (Type_Movement)br.ReadInt32(), Nation = (Folk)br.ReadInt32(),
                                Cell_Start = new Point(br.ReadInt32(), br.ReadInt32()),
                                Cell_Finish = new Point(br.ReadInt32(), br.ReadInt32()),
                                Slot = br.ReadInt32(), lvl = br.ReadInt32(),
                                ID = (Buildings)br.ReadInt32(), Destruction = br.ReadBoolean(),
                            };
                            //массивы загружать последними
                            for (int n = 0; n < MultiEvent.Units.Length; n++) {
                                MultiEvent.Units[n] = br.ReadInt32();/*11*/
                                if (n < MultiEvent.Farm.Length) MultiEvent.Farm[n] = br.ReadInt32();/*5*/
                            }
                            Stack.Add(MultiEvent);
                            LoadProcess.LoadText.Text = Text + $" Stack[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary> хранит всю необходимую информацию о событии. Является строкой в листе <b>List[TMultiEvent] Stack.</b>  </summary>
            /// <remarks> 
            ///     Доступ к полям осуществляется так: <br/>
            ///     <b> if (Event_Stack.IsMove(Point.X, Point.Y) == true) ... </b> <br/>
            ///     <b> Event_Stack.Stack[N].timer = 3 * 60 </b> <br/>
            ///     <b> Event_Stack.Stack[N].TypeEvent = Type_Event.RED_attack; </b> <br/>
            ///     <b> Event_Stack.Add(...); </b> <br/><br/>
            ///     Каждая деревня аккаунта может читать поля этого объекта <br/>
            ///     и выводить информацию на панель передвижения войск и на панель строительства.
            /// </remarks>
            public class TMultiEvent {
                /// <summary> Хранит время в секундах которое должно пройти до срабатывания события. Событие сработает и удалистя из стека когда timer = 0. </summary>
                public int timer = -1;
                /// <summary> Тип событий. По умолчанию - передвижение войск. </summary>
                public Type_Event TypeEvent = 0;
                /// <summary> Тип передвижений. По умолчанию: None. </summary>
                public Type_Movement TypeMovement = 0;
                /// <summary> Хранит название нации которая сгенерировала событие. </summary>
                public Folk Nation = Folk.NULL;
                /// <summary> Хранит координаты ячейки в которой началось событие в системе координат Cell[x][y]. </summary>
                public Point Cell_Start = new Point(0, 0);
                /// <summary> Хранит координаты ячейки в которой событие закончится в системе координат Cell[x][y] (если тип события = строительство, тогда Cell_Start = Cell_Finish = корам деревни сгенерировавшей событие). </summary>
                public Point Cell_Finish = new Point(0, 0);
                /// <summary> 
                ///     Массив на 11 элементов. Хранит кол-во юнитов каждого типа перемещаемых войск. <br/>
                ///     [0..9] - войска, [10] - герой. <br/>
                ///     (если тип события = строительство, тогда int[] Units = null). <br/>
                ///     Массив нужен для проведения симуляции боя и генерации отчёта.
                /// </summary>
                public int[] Units = new int[11];
                /// <summary> 
                ///     Массив на 5 элементов. Хранит кол-во добытых ресурсов каждого типа фармом: <br/>
                ///     <b>wood, clay, iron, crop, gold.</b> <br/>
                ///     (если тип события = строительство, тогда int[] Farm = null). <br/>
                ///     Массив нужен для генерации отчёта и добавления награбленного в хранилища.
                /// </summary>
                public int[] Farm = new int[5];

                /// <summary> Хранит номер слота, в котором идёт строительство. </summary>
                public int Slot = -1;
                /// <summary> Хранит уровень строящейся постройки. </summary>
                public int lvl = -1;
                /// <summary> Хранит название строящейся постройки. </summary>
                public Buildings ID = Buildings.ПУСТО;
                /// <summary> Хранит флаг постройки. true = постройка сностится, false = пострйока строится. </summary>
                public bool Destruction = false;

                public TMultiEvent() { }
                public TMultiEvent(int timer, Type_Event TypeEvent, Type_Movement TypeMovement, Folk Nation, Point Cell_Start, Point Cell_Finish, 
                                   int[] Units, int[] Farm, int Slot, int lvl, Buildings id, bool Destruction) {
                    if ((TypeEvent != Type_Event.Construction) && (Units.Length != this.Units.Length || Farm.Length != this.Farm.Length))
                        { MessageBox.Show($"Error 10.\nОшибка в Class TMultiEvent.\nОдин из переданных массивов имеет не верную длину.\nUnits.Length = {Units.Length}, а должно быть 11.\nFarm.Length = {Farm.Length}, а должно быть 5.\nВыход из программы."); Environment.Exit(1); return; }
                    this.timer = timer;    this.TypeEvent = TypeEvent;    this.TypeMovement = TypeMovement;
                    this.Nation = Nation;  this.Cell_Start = Cell_Start;  this.Cell_Finish = Cell_Finish;
                    this.Units = Units;    this.Farm = Farm;              this.Slot = Slot;
                    this.lvl = lvl;        ID = id;                       this.Destruction = Destruction;
                }

                /// <summary> Метод вычисляет максимальную скорость передвижения войск в переданном событии. <br/> Скорость всех юнитов выравнивается по наименьшей скорости отряда в Units[i]; </summary>
                /// <value> 
                ///     <b> <paramref name="Information"/>: </b> информация о войсках. <br/>
                ///     <b> <paramref name="Hero"/>: </b> герой игрока. <br/>
                /// </value>
                /// <returns> Возвращает максимально допустимую скорость передвижения войск переданного события (полей/в час). </returns>
                public int MoveSpeed(TTroops.TInformation[] Information, THero Hero) {
                    int MinSpeed = int.MaxValue;
                    for (int i = 0; i < Units.Length - 1; i++) {
                        if (Units[i] > 0) if (MinSpeed > Information[i].Speed) MinSpeed = (int)Information[i].Speed;
                    }
                    if (Units[10] > 0) if (MinSpeed > Hero.Speed) MinSpeed = Hero.Speed;
                    return MinSpeed;
                }

                /// <summary> Метод проверяет массив <b>Units[i] на наличие войск. </b> </summary>
                /// <returns> Возвращает <b>true</b> если есть хотя бы один воин. </returns>
                public bool IsTroops() {
                    if (Units == null) return false;
                    for (int i = 0; i < Units.Length; i++) if (Units[i] > 0) return true;
                    return false;
                }

                /// <summary> Метод разворачивает войска в стеке событий обратно к месту постоянной дислокации. </summary>
                /// <remarks> Это нужно если например деревню уничтодили или нужно развернуть нападающие войска обратно домой. </remarks>
                /// <value> Параметры метода пробрасываются из соответствующих объектов. </value>
                /// <returns> Возвращает <b>true</b> если разворот войск прошёл успешно. </returns>
                public bool ReverseMoveTroops(TTroops.TInformation[] Information, THero Hero, TMap Map) {
                    int Speed = MoveSpeed(Information, Hero);
                    double Dist = Map.Dist(new Point(Cell_Start.X, Cell_Start.Y),
                                           new Point(Cell_Finish.X, Cell_Finish.Y));
                    int Time = UFO.Convert.ToTimeSecond(Dist / Speed);//время передвижения войск в секундах
                    int Start_X = Cell_Start.X, Start_Y = Cell_Start.Y;
                    int Finish_X = Cell_Finish.X, Finish_Y = Cell_Finish.Y;
                        Cell_Start = new Point(Finish_X, Finish_Y);
                        Cell_Finish = new Point(Start_X, Start_Y);
                        timer = Time;
                        TypeEvent = Type_Event.REINFORCEMENTS;
                        TypeMovement = Type_Movement.Возвращение_войск;
                    return true;
                }

                //================================ ПЕРЕГРУЗКА  ОПЕРАТОРОВ ================================
                //======== Когда добавляю новые поля в этот класс, прописать их в этом блоке тоже ========
                //========================================================================================
                public static bool operator ==(TMultiEvent left, TMultiEvent right) {
                    if ((object)left == null) return (object)right == null; return left.Equals(right);
                }
                public static bool operator !=(TMultiEvent left, TMultiEvent right) { return !(left == right); }
                public override bool Equals(object obj) {
                    if (obj == null || GetType() != obj.GetType()) return false;
                    var right = (TMultiEvent)obj;
                    return (TypeEvent == right.TypeEvent && TypeMovement == right.TypeMovement &&
                        Nation == right.Nation && Cell_Start == right.Cell_Start && Cell_Finish == right.Cell_Finish &&
                        Units == right.Units && Farm == right.Farm && Slot == right.Slot && lvl == right.lvl &&
                        ID == right.ID && Destruction == right.Destruction);
                }
                public override int GetHashCode() {
                    return TypeEvent.GetHashCode() ^ TypeMovement.GetHashCode() ^ Nation.GetHashCode() ^
                           Cell_Start.GetHashCode() ^ Cell_Finish.GetHashCode() ^ Units.GetHashCode() ^
                           Farm.GetHashCode() ^ Slot.GetHashCode() ^ lvl.GetHashCode() ^ ID.GetHashCode() ^
                           Destruction.GetHashCode();
                }
                //================================ ПЕРЕГРУЗКА ОПЕРАТОРОВ ================================
            }
        }
        /// <summary>
        ///     Стек очереди событий. Хранит все события в игре.
        ///     бесконечный цикл System.Timer обрабатывает список событий <b> Event_Stack </b> в фоновом потоке
        ///     и реализовывает результат события, впоследствии удаляя отработанную строку-событие.
        /// </summary>
        public TEvent_Stack Event_Stack = null;
    }
}
