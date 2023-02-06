using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;

namespace GameLogica {
    public partial class TGame {
        /// <summary> содержит в себе необходимые поля и методы для реализации создания и хранения отчётов для вкладки "Отчёты". </summary>
        public class TReport {
            /// <summary>
            ///     Поле <b> LIST </b> хранит все возможные отчты в каждой строке. <br/>
            ///     <b> TData </b> - общий подкласс для всех возможных типов отчётов. <br/>
            ///     В каждой строке описан тип отчёта, отправитель/получатель отчёта (по координатам), заголовок отчёта, дата и т.д.
            /// </summary>
            public List<TData> LIST = new List<TData>();

            /// <summary> 
            ///     Метод добавляет строки в лист отчётов <b>List[TData] Report</b>. <br/> Каждая строка содержит отчёт. <br/><br/>
            ///     Значение: <br/>
            ///     <b> <paramref name="TypeReport"/>: </b> <inheritdoc cref="TData.TypeReport"/> <br/>
            ///     <b> <paramref name="TypeEvent"/>: </b> <inheritdoc cref="TData.TypeEvent"/> <br/>
            ///     <b> <paramref name="Archive"/>: </b> <inheritdoc cref="TData.Archive"/> <br/>
            ///     <b> <paramref name="Read"/>: </b> <inheritdoc cref="TData.Read"/> <br/>
            ///     <b> <paramref name="Cell_Start"/>: </b> <inheritdoc cref="TData.Cell_Start"/> <br/>
            ///     <b> <paramref name="Cell_Finish"/>: </b> <inheritdoc cref="TData.Cell_Finish"/> <br/>
            ///     <b> <paramref name="Date"/>: </b> <inheritdoc cref="TData.Date"/>. <br/>
            ///     <b> <paramref name="Time"/>: </b> <inheritdoc cref="TData.Time"/>. <br/>
            ///     <b> <paramref name="Attack_Troops"/>[]: </b> <inheritdoc cref="TData.Attack_Troops"/> <br/>
            ///     <b> <paramref name="Attack_Losses"/>[]: </b> <inheritdoc cref="TData.Attack_Losses"/> <br/>
            ///     <b> <paramref name="Defense_Village_Troops"/>[]: </b> <inheritdoc cref="TData.Defense_Village_Troops"/> <br/>
            ///     <b> <paramref name="Defense_Village_Losses"/>[]: </b> <inheritdoc cref="TData.Defense_Village_Losses"/> <br/>
            ///     <b> <paramref name="Defense_Troops"/>[]: </b> <inheritdoc cref="TData.Defense_Troops"/> <br/>
            ///     <b> <paramref name="Defense_Losses"/>[]: </b> <inheritdoc cref="TData.Defense_Losses"/> <br/>
            ///     <b> <paramref name="Units"/>[]: </b> <inheritdoc cref="TData.Units"/> <br/>
            ///     <b> <paramref name="Resources"/>[]: </b> <inheritdoc cref="TData.Resources"/> <br/>
            ///     <b> <paramref name="Wall_BeforeLevel"/>: </b> <inheritdoc cref="TData.Wall_BeforeLevel"/> <br/>
            ///     <b> <paramref name="Wall_AfterLevel"/>: </b> <inheritdoc cref="TData.Wall_AfterLevel"/> <br/>
            ///     <b> <paramref name="Construction_BeforeLevel_1"/>: </b> <inheritdoc cref="TData.Construction_BeforeLevel_1"/> <br/>
            ///     <b> <paramref name="Construction_AfterLevel_1"/>: </b> <inheritdoc cref="TData.Construction_AfterLevel_1"/> <br/>
            ///     <b> <paramref name="Construction_Name_1"/>: </b> <inheritdoc cref="TData.Construction_Name_1"/> <br/>
            ///     <b> <paramref name="Construction_TargetRandom_1"/>: </b> <inheritdoc cref="TData.Construction_TargetRandom_1"/> <br/>
            ///     <b> <paramref name="Construction_BeforeLevel_2"/>: </b> <inheritdoc cref="TData.Construction_BeforeLevel_2"/> <br/>
            ///     <b> <paramref name="Construction_AfterLevel_2"/>: </b> <inheritdoc cref="TData.Construction_AfterLevel_2"/> <br/>
            ///     <b> <paramref name="Construction_Name_2"/>: </b> <inheritdoc cref="TData.Construction_Name_2"/> <br/>
            ///     <b> <paramref name="Construction_TargetRandom_2"/>: </b> <inheritdoc cref="TData.Construction_TargetRandom_2"/> <br/>
            ///     <b> <paramref name="WarriorsFreedFromTraps_Count"/>: </b> <inheritdoc cref="TData.WarriorsFreedFromTraps_Count"/> <br/>
            ///     <b> <paramref name="WarriorsFreedFromTraps"/>: </b> <inheritdoc cref="TData.WarriorsFreedFromTraps"/> <br/>
            /// </summary>
            /// <remarks> Пример кода для добавления строк с отчётом в лист <b>Report</b> можно посмотреть в TGame.TEvent_Stack.Add(...); Метод заполняется аналогично. </remarks>
            /// <returns> Возвращает <b> true </b> если добавление прошло успешно. </returns>
            public bool Add(Type_Report TypeReport, Type_Event TypeEvent, bool Archive, bool Read, Point Cell_Start,
                            Point Cell_Finish, string Date, string Time, int[] Attack_Troops, int[] Attack_Losses,
                            int[] Defense_Village_Troops, int[] Defense_Village_Losses, int[] Defense_Troops,
                            int[] Defense_Losses, int[] Units, int[] Resources, int Wall_BeforeLevel, int Wall_AfterLevel,
                            int Construction_BeforeLevel_1, int Construction_AfterLevel_1,
                            Buildings Construction_Name_1, bool Construction_TargetRandom_1,
                            int Construction_BeforeLevel_2, int Construction_AfterLevel_2, 
                            Buildings Construction_Name_2, bool Construction_TargetRandom_2,
                            int WarriorsFreedFromTraps_Count, Traps WarriorsFreedFromTraps) {
                //на внешнем контуре сюда отправляю null если это не атака/набег, но массивы всё равно создаются.
                if (Attack_Troops == null) Attack_Troops = new int[11];
                if (Attack_Losses == null) Attack_Losses = new int[11];
                if (Defense_Village_Troops == null) Defense_Village_Troops = new int[11];
                if (Defense_Village_Losses == null) Defense_Village_Losses = new int[11];
                if (Defense_Troops == null) Defense_Troops = new int[55];
                if (Defense_Losses == null) Defense_Losses = new int[55];
                if (Units == null) Units = new int[11];
                if (Resources == null) Resources = new int[5];
                LIST.Add(new TData(TypeReport, TypeEvent, Archive, Read, Cell_Start, Cell_Finish, Date, Time,
                           Attack_Troops, Attack_Losses, Defense_Village_Troops, Defense_Village_Losses,
                           Defense_Troops, Defense_Losses, Units, Resources, Wall_BeforeLevel, Wall_AfterLevel,
                           Construction_BeforeLevel_1, Construction_AfterLevel_1, 
                           Construction_Name_1, Construction_TargetRandom_1,
                           Construction_BeforeLevel_2, Construction_AfterLevel_2,
                           Construction_Name_2, Construction_TargetRandom_2,
                           WarriorsFreedFromTraps_Count, WarriorsFreedFromTraps));
                return true;
            }

            /// <summary>
            ///     Метод сохраняет содержимое всех строк поля TReport.LIST в файл: <b>Reports.DAT</b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void SaveReports(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Reports.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(LIST.Count);/*0*/ //сохраняем количество строк в листе отчётов первым символом
                        if (LIST.Count <= 0) return;
                        for (int i = 0; i < LIST.Count; i++) {
                            bw.Write((int)LIST[i].TypeReport);       bw.Write((int)LIST[i].TypeEvent); 
                            bw.Write(LIST[i].Archive);               bw.Write(LIST[i].Read);           
                            bw.Write(LIST[i].Cell_Start.X);          bw.Write(LIST[i].Cell_Start.Y);   
                            bw.Write(LIST[i].Cell_Finish.X);         bw.Write(LIST[i].Cell_Finish.Y);  
                            bw.Write(LIST[i].Date);                  bw.Write(LIST[i].Time);
                            bw.Write(LIST[i].Wall_BeforeLevel);           bw.Write(LIST[i].Wall_AfterLevel);
                            //цель для катапульт №1
                            bw.Write(LIST[i].Construction_BeforeLevel_1);
                            bw.Write(LIST[i].Construction_AfterLevel_1);
                            bw.Write((int)LIST[i].Construction_Name_1);
                            bw.Write(LIST[i].Construction_TargetRandom_1);
                            //цель для катапульт №2
                            bw.Write(LIST[i].Construction_BeforeLevel_2);
                            bw.Write(LIST[i].Construction_AfterLevel_2);
                            bw.Write((int)LIST[i].Construction_Name_2);
                            bw.Write(LIST[i].Construction_TargetRandom_2);

                            bw.Write(LIST[i].WarriorsFreedFromTraps_Count);
                            bw.Write((int)LIST[i].WarriorsFreedFromTraps);
                            //массивы сохранять последними
                            for (int j = 0; j < LIST[i].Attack_Troops.Length; j++) {   //11 элементов
                                bw.Write(LIST[i].Attack_Troops[j]); 
                                bw.Write(LIST[i].Attack_Losses[j]);
                                bw.Write(LIST[i].Defense_Village_Troops[j]);
                                bw.Write(LIST[i].Defense_Village_Losses[j]);
                                bw.Write(LIST[i].Units[j]);
                            }
                            for (int j = 0; j < LIST[i].Defense_Troops.Length; j++) {  //55 элементов
                                bw.Write(LIST[i].Defense_Troops[j]); bw.Write(LIST[i].Defense_Losses[j]);
                            }
                            for (int j = 0; j < LIST[i].Resources.Length; j++) {  //5 элементов
                                bw.Write(LIST[i].Resources[j]);
                            }

                            LoadProcess.LoadText.Text = Text + $" Report[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary>
            ///     Метод загружает в лист отчётов <b>TReport.LIST</b> данные из бинарного файла <b> Reports.DAT </b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void LoadReports(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Reports.DAT", FileMode.Open)) {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                        int Report_Count = br.ReadInt32();/*0*/ if (Report_Count <= 0) return;
                        for (int i = 0; i < Report_Count; i++) {
                            var Data = new TData() { 
                                TypeReport = (Type_Report)br.ReadInt32(), TypeEvent = (Type_Event)br.ReadInt32(),
                                Archive = br.ReadBoolean(),               Read = br.ReadBoolean(),
                                Cell_Start = new Point(br.ReadInt32(), br.ReadInt32()),
                                Cell_Finish = new Point(br.ReadInt32(), br.ReadInt32()),
                                Date = br.ReadString(),                   Time = br.ReadString(),
                                Wall_BeforeLevel = br.ReadInt32(),        Wall_AfterLevel = br.ReadInt32(),
                                //цель для катапульт №1
                                Construction_BeforeLevel_1 = br.ReadInt32(),
                                Construction_AfterLevel_1 = br.ReadInt32(),
                                Construction_Name_1 = (Buildings)br.ReadInt32(),
                                Construction_TargetRandom_1 = br.ReadBoolean(),
                                //цель для катапульт №2
                                Construction_BeforeLevel_2 = br.ReadInt32(),
                                Construction_AfterLevel_2 = br.ReadInt32(),
                                Construction_Name_2 = (Buildings)br.ReadInt32(),
                                Construction_TargetRandom_2 = br.ReadBoolean(),

                                WarriorsFreedFromTraps_Count = br.ReadInt32(),
                                WarriorsFreedFromTraps = (Traps)br.ReadInt32(),
                            };
                            //массивы загружать последними
                            for (int j = 0; j < Data.Attack_Troops.Length; j++) {   //11 элементов
                                Data.Attack_Troops[j] = br.ReadInt32();
                                Data.Attack_Losses[j] = br.ReadInt32();
                                Data.Defense_Village_Troops[j] = br.ReadInt32();
                                Data.Defense_Village_Losses[j] = br.ReadInt32();
                                Data.Units[j] = br.ReadInt32();
                            }
                            for (int j = 0; j < Data.Defense_Troops.Length; j++) {  //55 элементов
                                Data.Defense_Troops[j] = br.ReadInt32();
                                Data.Defense_Losses[j] = br.ReadInt32();
                            }
                            for (int j = 0; j < Data.Resources.Length; j++) {  //5 элементов
                                Data.Resources[j] = br.ReadInt32();
                            }
                            
                            LIST.Add(Data);
                            LoadProcess.LoadText.Text = Text + $" Report[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary> хранит всю необходимую информацию об отчёте. Является строкой в листе <b>TReport.LIST[TData];</b> </summary>
            /// <remarks> 
            ///     Доступ к полям осуществляется так: <br/>
            ///     <b> TReport.LIST[N].TypeReport = Type_Report.Attack_Win_GREEN; </b> <br/>
            ///     <b> TReport.LIST[N].Date = "16.11.2022"; </b> <br/>
            ///     <b> TReport.Add(...); </b> <br/><br/>
            ///     Читать поля этого объекта может только игровой аккаунт <b>Player</b>.
            /// </remarks>
            public class TData {
                /// <summary> Тип отчёта. По умолчанию - Attack_Win_GREEN. </summary>
                /// <remarks> Путь к пиктограммам, имена файлов и текст заголовка отчёта для таблицы определяются из этого поля. </remarks>
                public Type_Report TypeReport = 0;
                /// <summary> Дополнительный тип отчёта. По умолчанию - ATTACK (нападение). </summary>
                /// <remarks> Нападение, набег, подкрепление, расселение, захват, приключение и т.д. </remarks>
                public Type_Event TypeEvent = 0;
                /// <summary> Флаг статуса отчёта. true = отчёт помещён в архив, false = отчёт не в архиве. </summary>
                public bool Archive = false;
                /// <summary> Флаг статуса отчёта. true = отчёт прочитан, false = отчёт не прочитан. </summary>
                public bool Read = false;
                /// <summary> Хранит координаты ячейки которая является отправителем отчёта в системе координат Cell[x][y]. </summary>
                /// <remarks> Тип отчёта хранит опосредованную информацию о том кто является принимающей стороной - старт или финиш. </remarks>
                public Point Cell_Start = new Point(0, 0);
                /// <summary> Хранит координаты ячейки которая является адресатом в системе координат Cell[x][y]. </summary>
                /// <remarks> Тип отчёта хранит опосредованную информацию о том кто является принимающей стороной - старт или финиш. </remarks>
                public Point Cell_Finish = new Point(0, 0);
                /// <summary> Дата сгенерированного отчёта. Тип: ДД.ММ.ГГ. Ячейка таблицы: [4] </summary>
                public string Date;
                /// <summary> Время сгенерированного отчёта. Тип: ч:м:с. Ячейка таблицы: [4] </summary>
                public string Time;
                /// <summary> Массив на 11 элементов. <br/>
                ///     - Хранит кол-во <b>нападающих</b> воинов каждого типа (войска). <br/>
                ///     - Хранит количество размещённых воинов в качестве подкрепа в другой деревне. <br/>
                ///     - Хранит количество поселенцев, которые основали новую деревню
                /// </summary>
                /// <remarks> <b>[0..9]</b> = войска <br/> <b>[10]</b> = герой </remarks>
                public int[] Attack_Troops = new int[11];
                /// <summary> Массив на 11 элементов. Хранит кол-во <b>убитых</b> нападающих воинов каждого типа (потери). <br/> а так же хранит количество убитых воинов, стоящих подкрепом в другой деревне. </summary>
                /// <remarks> <inheritdoc  cref="Attack_Troops"/> </remarks>
                public int[] Attack_Losses = new int[11];
                /// <summary> Массив на 11 элементов. ВОЙСКА ДЕРЕВНИ. Хранит кол-во <b>обороняющихся</b> воинов каждого типа принадлежащих деревне (войска). </summary>
                /// <remarks> <inheritdoc  cref="Attack_Troops"/> </remarks>
                public int[] Defense_Village_Troops = new int[11];
                /// <summary> Массив на 11 элементов. ВОЙСКА ДЕРЕВНИ. Хранит кол-во <b>убитых</b> обороняющихся воинов каждого типа принадлежащих деревне (потери). </summary>
                /// <remarks> <inheritdoc  cref="Attack_Troops"/> </remarks>
                public int[] Defense_Village_Losses = new int[11];
                /// <summary> Массив на 55 элементов ПОДКРЕП. Хранит кол-во <b>обороняющихся</b> воинов каждого типа включая героя для каждой нации. </summary>
                /// <remarks>
                ///     [0..10] = Римляне / Romans [11] <br/> [11..21] = Германцы / Germans [11] <br/>
                ///     [22..32] = Галлы / Gauls [11] <br/> [33..43] = Дикая природа / Nature [11] <br/>
                ///     [44..54] = Натары / Natars [11] <br/><br/> [10, 21, 32, 43, 54] = Герои / Heroes [5]
                /// </remarks>
                public int[] Defense_Troops = new int[55];
                /// <summary> Массив на 55 элемент. ПОДКРЕП. Хранит кол-во <b>убитых</b> обороняющихся воинов каждого типа включая героя для каждой нации. </summary>
                /// <remarks> <inheritdoc cref="Defense_Troops"/> </remarks>
                public int[] Defense_Losses = new int[55];
                /// <summary> Массив на 11 элементов. Хранит кол-во выживших юнитов после атаки <b>АТАКУЮЩЕЙ</b> стороной, в том числе и стоящих в подкрепе (Attack_Troops[] - Attack_Losses[]). </summary>
                /// <remarks> <inheritdoc  cref="Attack_Troops"/> </remarks>
                public int[] Units = new int[11];
                /// <summary> Массив на 5 элементов. Хранит добытые ресурсы атакующей стороной (добыча) и ресурсы отправленных торговцев. </summary>
                /// <remarks> [0] wood [1] clay [2] iron [3] crop [4] gold <br/> Сумма добытого и сумма вместимости ресурсов вычисляются из данных отчёта перед вызовом окна отчёта. </remarks>
                public int[] Resources = new int[5];
                /// <summary> Информация об уровне стены <b>ДО</b> нанесения урона таранами. </summary>
                /// <remarks> Пример: <br/> - Городская стена разрушена с уровня 20 до уровня 13 <br/> - Земляной вал разрушен полностью <br/> Изгородь не была повреждена <br/> Стена отсутствует (-1) </remarks>
                public int Wall_BeforeLevel;
                /// <summary> Информация об уровне стены <b>ПОСЛЕ</b> нанесения урона таранами. </summary>
                /// <remarks> Пример: <br/> - Городская стена разрушена с уровня 20 до уровня 13 <br/> - Земляной вал разрушен полностью <br/> Изгородь не была повреждена <br/> Стена отсутствует (-1) </remarks>
                public int Wall_AfterLevel;
                /// <summary> Информация об уровне здания <b>ДО</b> нанесения урона катапультами. <b>Строка 1/2.</b> </summary>
                /// <remarks>
                ///     Если катапульты нацелены только на одну цель, вторая пара переменных должна содержать: -1. <br/>
                ///     Если в атаке катапульты не участвуют, все 4 переменных должны содержать: -1. <br/>
                ///     Пример: <br/> - Главное здание разрушено с уровня 12 до уровня 3. <br/> - Тайник разрушен полностью.
                /// </remarks>
                public int Construction_BeforeLevel_1;
                /// <summary> Информация об уровне здания <b>ПОСЛЕ</b> нанесения урона катапультами. <b>Строка 1/2.</b> </summary>
                /// <remarks> <inheritdoc cref="Construction_BeforeLevel_1"/> </remarks>
                public int Construction_AfterLevel_1;
                /// <summary> Информация о названии здания по которому был произведён залп катапультами. <b>Строка 1/2.</b> </summary>
                public Buildings Construction_Name_1 = Buildings.ПУСТО;
                /// <summary> Флаг случайной цели. <b>true</b> = случайная цель, <b>false</b> = нет. <b>Строка 1/2.</b> </summary>
                public bool Construction_TargetRandom_1 = false;
                /// <summary> Информация об уровне здания <b>ДО</b> нанесения урона катапультами. <b>Строка 2/2.</b> </summary>
                /// <remarks> <inheritdoc cref="Construction_BeforeLevel_1"/> </remarks>
                public int Construction_BeforeLevel_2;
                /// <summary> Информация об уровне здания <b>ПОСЛЕ</b> нанесения урона катапультами. <b>Строка 2/2.</b> </summary>
                /// <remarks> <inheritdoc cref="Construction_BeforeLevel_1"/> </remarks>
                public int Construction_AfterLevel_2;
                /// <summary> Информация о названии здания по которому был произведён залп катапультами. <b>Строка 2/2.</b> </summary>
                public Buildings Construction_Name_2 = Buildings.ПУСТО;
                /// <summary> Флаг случайной цели. <b>true</b> = случайная цель, <b>false</b> = нет. <b>Строка 2/2.</b> </summary>
                public bool Construction_TargetRandom_2 = false;
                /// <summary> Общее кол-во воинов освобождённых из капканов галлов (свои/союзные/все). -1, если освобождённых нет. </summary>
                public int WarriorsFreedFromTraps_Count;
                /// <summary> Информация об освобождённых воинах из капканов галлов (свои/союзные/все). По умолчанию <b>Traps.None</b>. </summary>
                public Traps WarriorsFreedFromTraps = Traps.None;

                public TData() { }
                public TData(Type_Report TypeReport, Type_Event TypeEvent, bool Archive, bool Read, Point Cell_Start,
                                   Point Cell_Finish, string Date, string Time, int[] Attack_Troops,
                                   int[] Attack_Losses, int[] Defense_Village_Troops, int[] Defense_Village_Losses,
                                   int[] Defense_Troops, int[] Defense_Losses, int[] Units, int[] Resources,
                                   int Wall_BeforeLevel, int Wall_AfterLevel,
                                   int Construction_BeforeLevel_1, int Construction_AfterLevel_1,
                                   Buildings Construction_Name_1, bool Construction_TargetRandom_1,
                                   int Construction_BeforeLevel_2, int Construction_AfterLevel_2,
                                   Buildings Construction_Name_2, bool Construction_TargetRandom_2,
                                   int WarriorsFreedFromTraps_Count, Traps WarriorsFreedFromTraps) {
                    if (Attack_Troops.Length != this.Attack_Troops.Length || Attack_Losses.Length != this.Attack_Losses.Length ||
                        Defense_Village_Troops.Length != this.Defense_Village_Troops.Length || Defense_Village_Losses.Length != this.Defense_Village_Losses.Length ||
                        Defense_Troops.Length != this.Defense_Troops.Length || Defense_Losses.Length != this.Defense_Losses.Length ||
                        Units.Length != this.Units.Length || Resources.Length != this.Resources.Length) {
                        MessageBox.Show("Error 23.\nОшибка в Class TReport.TData;\nОдин из переданных массивов имеет не верную длину.\n" +
                            "Название массива = фактический размер/верный размер:\n" +
                            $"Attack_Troops = {Attack_Troops.Length}/11.\nAttack_Losses = {Attack_Losses.Length}/11.\n" +
                            $"Defense_Village_Troops = {Defense_Village_Troops.Length}/11.\nDefense_Village_Losses = {Defense_Village_Losses.Length}/11.\n" +
                            $"Defense_Troops = {Defense_Troops.Length}/55.\nDefense_Losses = {Defense_Losses.Length}/55.\n" +
                            $"Units = {Units.Length}/11.\nResources = {Resources.Length}/5.\nВыход из программы."); Environment.Exit(1); return; }
                    this.TypeReport = TypeReport;          this.TypeEvent = TypeEvent;
                    this.Archive = Archive;                this.Read = Read;          
                    this.Cell_Start = Cell_Start;          this.Cell_Finish = Cell_Finish;
                    this.Date = Date;                      this.Time = Time;
                    this.Attack_Troops = Attack_Troops;    this.Attack_Losses = Attack_Losses;
                    this.Defense_Village_Troops = Defense_Village_Troops;
                    this.Defense_Village_Losses = Defense_Village_Losses;
                    this.Defense_Troops = Defense_Troops;  this.Defense_Losses = Defense_Losses;
                    this.Units = Units;                    this.Resources = Resources;          
                    this.Wall_BeforeLevel = Wall_BeforeLevel; this.Wall_AfterLevel = Wall_AfterLevel;
                    //цель для катапульт №1
                    this.Construction_BeforeLevel_1 = Construction_BeforeLevel_1;
                    this.Construction_AfterLevel_1 = Construction_AfterLevel_1;
                    this.Construction_Name_1 = Construction_Name_1;
                    this.Construction_TargetRandom_1 = Construction_TargetRandom_1;
                    //цель для катапульт №2
                    this.Construction_BeforeLevel_2 = Construction_BeforeLevel_2; 
                    this.Construction_AfterLevel_2 = Construction_AfterLevel_2;
                    this.Construction_Name_2 = Construction_Name_2;
                    this.Construction_TargetRandom_2 = Construction_TargetRandom_2;

                    this.WarriorsFreedFromTraps_Count = WarriorsFreedFromTraps_Count;
                    this.WarriorsFreedFromTraps = WarriorsFreedFromTraps;
                }
            }
        }
        /// <summary> Объект отчётов. Хранит список всех сгенерированных отчётов связанных с игроком. </summary>
        public TReport Reports = null;
    }
}
