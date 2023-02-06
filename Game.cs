using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame.TPlayer;

namespace GameLogica {
    /// <summary> 
    ///     Класс <b>TGame.</b> Содержит поля, подклассы и методы для реализации игры. <br/><br/>
    ///     <b> Подклассы TGame: </b> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TAlliance: </b> <inheritdoc cref="TAlliance"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TPlayer: </b> содержит в себе всю не визуальную часть игры кроме логики. <br/>
    ///     <paramref name="class"/> <b> TGame.TPlayer.THero: </b> <inheritdoc cref="THero"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TPlayer.TVillage: </b> <inheritdoc cref="TVillage"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TPlayer.TVillage.TSlot: </b> <inheritdoc cref="TVillage.TSlot"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TPlayer.TVillage.Reinforcements_Of_Troops: </b> <inheritdoc cref="TVillage.Reinforcements_Of_Troops"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TEvent_Stack: </b> <inheritdoc cref="TEvent_Stack"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TEvent_Stack.TMultiEvent: </b> <inheritdoc cref="TEvent_Stack.TMultiEvent"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TMessage: </b> <inheritdoc cref="TMessage"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TMessage.TData: </b> <inheritdoc cref="TMessage.TData"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TReport: </b> <inheritdoc cref="TReport"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TReport.TData: </b> <inheritdoc cref="TReport.TData"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TTroops: </b> <inheritdoc cref="TTroops"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TTroops.TInformation: </b> <inheritdoc cref="TTroops.TInformation"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TBuild: </b> <inheritdoc cref="TBuild"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TBuild.TInformation: </b> <inheritdoc cref="TBuild.TInformation"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.TMap: </b> <inheritdoc cref="TMap"/> <br/>
    ///     <paramref name="class"/> <b> TGame.TMap.TCell: </b> <inheritdoc cref="TMap.TCell"/> <br/><br/>
    ///     <paramref name="class"/> <b> TGame.FILE: </b> static class, содержит static методы для работы с файлами. Создание экземпляра класса не требуется. <br/>
    /// </summary>
    public partial class TGame {
        //private NumberFormatInfo provider = new NumberFormatInfo();//для конвертации Double.toString чисел с точками

        //поля загружаются из файла Interface.DAT
        /// <summary> Возвращает или задаёт логический флаг всплывающих подсказкок для класса <b> TPlayer. </b> <br/>
        ///           <b> true </b> - разрешает показывать всплывающие подсказки в этом аккаунте / <b> false </b> - запрещает. </summary>
        private bool tooltipflag = true;
        /// <summary> <inheritdoc cref = "tooltipflag"> </inheritdoc> </summary>
        public bool ToolTipFlag { get { return tooltipflag; } set { tooltipflag = value; } }
        /// <summary> Возвращает или задаёт логический флаг цветовой дифференциации войск на панели <b>"Войска"</b> в зависимости от нации. <br/>
        ///           <b> true </b> - разноцветные величины войск / <b> false </b> - все нации печатать чёрным цветом. </summary>
        private bool colorarmyflag = true;
        /// <summary> <inheritdoc cref = "colorarmyflag"> </inheritdoc> </summary>
        public bool ColorArmyFlag { get { return colorarmyflag; } set { colorarmyflag = value; } }
        /// <summary> Возвращает или задаёт язык интерфейса. default = 0 </summary>
        private int language = 0;
        /// <summary> <inheritdoc cref = "language"> </inheritdoc> </summary>
        public int Language { get { return language; } set { language = value; } }
        /// <summary> Хранит величину скорости игры в милисекундах. По умолчанию поле = 1000 мс (1 сек). </summary>
        private int speed_game = 1000;
        /// <summary> <inheritdoc cref = "speed_game"> </inheritdoc> </summary>
        public int SpeedGame { get { return speed_game; } set { speed_game = value; } }
        /// <summary> Возвращает или задаёт номер фоновой картинки на вкладке <b>"Деревня"</b>. <br/> </summary>
        private int bgvillage_numberimage = 0;
        /// <summary> <inheritdoc cref = "bgvillage_numberimage"> </inheritdoc> </summary>
        public int bgVillage_NumberImage { get { return bgvillage_numberimage; } set { bgvillage_numberimage = value; } }

        //поля не требующие сохранения в файл
        /// <summary> Метод-пустышка. Нужен только как ссылка на тег <b>remarks</b> </summary>
        /// <remarks> Если значение из <b>json</b> файла не прочиталось, </remarks>
        private void JSON() { }
        /// <summary> Хранит относительный путь папки, в которую сохраняется игра. <br/> Например: <b>DATA_BASE/ACCOUNTS</b> <br/> слэш "/" в конце не ставится. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>PathFolderSave</b> = "DATA_BASE/ACCOUNTS"; </remarks> 
        public string PathFolderSave = "DATA_BASE/ACCOUNTS";
        /// <summary> Хранит максимальное разрешённое количество одновременно запущенных построек у наций. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>MaxCount_Building_InOfConstructionQueue</b> = null; </remarks> 
        public string[,] MaxCount_Building_InOfConstructionQueue = null;// { Римляне = 4, Германцы = 3, Галлы = 2}
        /// <summary> Хранит двумерный массив [3, 11] бонусных значений (%) от оазисов. Значения грузятся из файла один раз в конструкторе. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>Bonuses_Oasis</b> = null; </remarks> 
        public string[,] Bonuses_Oasis = null;
        /// <summary> Хранит двумерный массив [125, 2] культуры. Содержит 2 колонки, левая = кол-во деревень, правая = величина культуры которая соответствует этому количеству деревень. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>ListOfCulture[,]</b> = null; </remarks> 
        //public string[,] ListOfCulture = null;
        /// <summary> Возвращает или задаёт базовую вместимость всех хранилищ аккаунта. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>Capacity_Basic</b> = default(0, 0, 0, 0, 0); </remarks> 
        public sCapacity Capacity_Basic;
        /// <summary> РЫНОК <br/> Возвращает или задаёт базовую грузоподъёмность торговцев для разных наций. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>BasicLoadCapacityOfMerchants_Market</b> = default(0, 0, 0); </remarks> 
        public BasicParameterOfNation BasicLoadCapacityOfMerchants_Market;
        /// <summary> РЫНОК <br/> Возвращает или задаёт базовую скорость торговцев для разных наций. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>BasicSpeedOfMerchants_Market</b> = default(0, 0, 0); </remarks> 
        public BasicParameterOfNation BasicSpeedOfMerchants_Market;
        /// <summary> РАТУША <br/> Возвращает или задаёт стоимость маленького праздника в ресурсах. <br/> Продолжительность праздника в свойствах постройки: парам1, парам2. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>CostHoliday_TownHall</b> = default(0, 0, 0, 0, 0); </remarks> 
        public Res CostHoliday_TownHall;
        /// <summary> РАТУША <br/> Возвращает или задаёт стоимость торжества в ресурсах. <br/> Продолжительность праздника в свойствах постройки: парам1, парам2. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>CostCelebrations_TownHall</b> = default(0, 0, 0, 0, 0); </remarks> 
        public Res CostCelebrations_TownHall;
        /// <summary> Возвращает или задаёт стоимость праздника (Германцы) в ресурсах. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>CostHoliday_BreweryGermans</b> = default(0, 0, 0, 0, 0); </remarks> 
        public Res CostHoliday_BreweryGermans;
        /// <summary> Возвращает или задаёт продолжительность праздника (Германцы) в секундах. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>DurationHoliday_BreweryGermans</b> = 0; </remarks> 
        public int DurationHoliday_BreweryGermans;
        /// <summary> Возвращает или задаёт стоимость одного капкана у галлов в ресурсах. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>CostOneTrap_TrapperGauls</b> = 0; </remarks> 
        public Res CostOneTrap_TrapperGauls;
        /// <summary> Возвращает или задаёт продолжительность ремонта одного капкана у галлов в секундах. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>TimeRepairOneTrap_TrapperGauls</b> = 0; </remarks> 
        public int TimeRepairOneTrap_TrapperGauls;
        /// <summary> 
        ///     Возвращает или задаёт базовое время создания одного капкана у галлов в секундах на 1 уровне здания. <br/>
        ///     Базовое время сокращается по мере роста уровня Капканщика так же как и для казармы. <br/>
        ///     По этмоу бонус времени для этого поля можно подсмотреть для аналогичного уровня казармы.
        /// </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>BaseTimeCreatingNewTrap_TrapperGauls</b> = 0; </remarks> 
        public int BaseTimeCreatingNewTrap_TrapperGauls;
        /// <summary> Возвращает или задаёт максимальное количество символов для строки с названием аккаунта и деревни. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>Name_MaxLength</b> = 20; </remarks> 
        public int Name_MaxLength = 20;
        /// <summary> Возвращает или задаёт максимальное количество символов для строки с названием темы сообщения. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>Name_MaxLength</b> = 20; </remarks> 
        public int Topic_MaxLength = 30;
        /// <summary> Стоимость в золоте за моментальное завершение строительства. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>CostInGoldForInstantCompletionOfConstruction</b> = 1; </remarks> 
        public int CostInGoldForInstantCompletionOfConstruction = 1;
        /// <summary> Радиус окрестности вокруг активной деревни игрока (для отчётов), если значение из json не прочиталось, по умолчание Neighborhood = 3. <br/> Центр окрестности в расчёт не берётся, т.е. диаметр 3 + 3 = 7, т.к. +1 от центра в которой расположена деревня игрока. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>Radius_Neighborhood</b> = 3; </remarks> 
        public int Radius_Neighborhood = 3;
        /// <summary> Максимальное количество строк отображаемых деревень в таблице окна профиля игрока. </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>MaxVillageTabelRowsVisibleProfileWindow</b> = 10; </remarks> 
        public int MaxVillageTabelRowsVisibleProfileWindow = 10;
        /// <summary> Скорость амплитудной анимации сворачивания/разворачивания информации в окне "профиль игрока". </summary>
        /// <remarks> <inheritdoc cref="JSON"/> по умолчанию <b>SpeedAnimateProfileWindow</b> = 0; (без анимации) </remarks> 
        public int SpeedAnimateProfileWindow = 0;

        /// <summary> конструктор TGame. <br/> корректно создаёт экземпляр класса. Заполняет информацией на старте нужные поля. <br/> Некоторые дочерние подклассы создаются в этом конструкторе. </summary>
        public TGame() {
            //здесь не вызывать потому что на старте в TGame грузится файл json который создаёт объект со своими свойствами
            //из файла. всё что я тут прочту из .DAT всё равно заменится на json. этот код вызывается в Form1_Load()
            //if (File.Exists("DATA_BASE/ACCOUNTS/Interface.DAT")) LoadInterface();
            for (int i = 0; i < Build.Length; i++) {
                Build[i] = new TBuild();//создание массива всех построек
                for (int j = 0; j < Build[0].Information.Length; j++) Build[i].Information[j] = new TBuild.TInformation();
            }
            Load_All_Buildings();//загрузить все постройки в Build
            for (int i = 0; i < Troops.Length; i++) {
                Troops[i] = new TTroops();//создание массива всех наций
                for (int j = 0; j < Troops[i].Information.Length; j++) Troops[i].Information[j] = new TTroops.TInformation();
            }
            Load_All_Troops();//загрузить все войска в Troops
            Event_Stack = new TEvent_Stack();
            Reports = new TReport();
            Messages = new TMessage();
            Alliances = new TAlliance();
            Bonuses_Oasis = Newtonsoft.Json.JsonConvert.DeserializeObject<string[,]>
                (File.ReadAllText("DATA_BASE/JSON/Bonuses_Oasis.json"));
            //загрузка этого json прописана в Player.Check_Limit_Village();
            //ListOfCulture = Newtonsoft.Json.JsonConvert.DeserializeObject<string[,]>
              //  (File.ReadAllText("DATA_BASE/JSON/culture_for_new_village_TRAVIAN(x3).json"));
             MaxCount_Building_InOfConstructionQueue = Newtonsoft.Json.JsonConvert.DeserializeObject<string[,]>
                (File.ReadAllText("DATA_BASE/JSON/MaxCountConstructions.json"));
        }


        /// <summary> 
        ///     Метод очищает папку сэйва чтобы создать новый и старые имена файлов не смешались с новыми. <br/> <br/>
        ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
        /// </summary>
        /// <returns> Возвращает <b>true</b> если очистка прошла успешно. </returns>
        public bool ClearSaves(string path) {
            string[] PathArray = Directory.GetDirectories($"{path}/");
            for (int i = 0; i < PathArray.Length; i++) Directory.Delete(PathArray[i], true);
            File.Delete($"{path}/Event_Stack.DAT");
            File.Delete($"{path}/Messages.DAT");
            File.Delete($"{path}/Reports.DAT");
            return true;
        }

        /// <summary> 
        ///     Метод сохраняет в бинарный файл <b>Interface.DAT</b> значения своих полей. <br/> <br/>
        ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
        /// </summary>
        public void SaveInterface(string path) {
            using (FileStream fs = new FileStream($"{path}/Interface.DAT", FileMode.Create)) {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                    bw.Write(tooltipflag);    bw.Write(ColorArmyFlag);
                    bw.Write(Language);       bw.Write(SpeedGame);
                    bw.Write(bgVillage_NumberImage);
                }
            }
        }

        /// <summary> 
        ///     Метод загружает из бинарного файла <b>Interface.DAT</b> значения в свои поля. <br/> <br/>
        ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
        /// </summary>
        public void LoadInterface(string path) {
            using (FileStream fs = new FileStream($"{path}/Interface.DAT", FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                    ToolTipFlag = br.ReadBoolean();    ColorArmyFlag = br.ReadBoolean();
                    Language = br.ReadInt32();         SpeedGame = br.ReadInt32();
                    bgVillage_NumberImage = br.ReadInt32();
                }
            }
        }

        /// <summary> 
        ///     Метод возвращает разрешённое максимальное количество одновременно строящихся построек в очереди строительства. <br/>
        ///     Сведения о разрешённом количестве одновременных построек для разных наций хранится в файле
        ///     <b>DATA_BASE/JSON/MaxCountConstructions.json</b> 
        /// </summary>
        /// <value> <b>Folk_Name:</b> национальность аккаунта. </value>
        /// <returns> Возвращает разрешённое максимальное количество одновременно строящихся построек. </returns>
        public int Get_MaxCount_Building_InOfConstructionQueue(Folk Folk_Name) {
            return (int)Folk_Name <= -1 || Folk_Name == Folk.NULL || Folk_Name > Folk.NULL ? -1
                   : Convert.ToInt32(MaxCount_Building_InOfConstructionQueue[(int)Folk_Name, 1]);
        }

        /// <summary> Метод проверяет числовой массив в заданном диапазоне на значения больше нуля. <br/> При дефолтных значениях, будут проверены все элементы массива. </summary>
        /// <value>
        ///     <b>Index:</b> начало перебора элементов, по умолчанию [0]. <br/>
        ///     <b>Length:</b> длина перебора элементов, по умолчанию [Array.Length]. <br/>
        /// </value>
        /// <returns> Возвращает <b>true</b> если есть хотя бы одно число больше нуля в заданном диапазоне. </returns>
        public bool IsNotZero_Array(int[] Array, int Index = 0, int Length = 0) {
            if (Array == null) return false;
            else { if (Length == 0) Length = Array.Length;
                for (int i = Index; i < Index + Length; i++) if (Array[i] > 0) return true;
            } return false;
        }

        /// <summary> Метод конвертирует <b>enum TypeCell</b> в эквивалент строки. </summary>
        /// <value>
        ///     <b><paramref name="TypeCell"/>:</b> поле <b>TVillage.Type_Resources;</b> тип ячейки в <b>Game.Map.Cell[,];</b> <br/>
        ///     <b><paramref name="LANG"/>:</b> словарик с локализацией. <br/>
        /// </value>
        /// <returns> Возвращает строку формата: <br/> [древесина: N; глина: N; железо: N; зерно: N], <br/> где <b>N</b> зависит от типа <b>TypeCell</b>. </returns>
        public string TypeCellToString(TypeCell TypeCell, WindowsInterface.Form1.TLANGUAGES LANG) {
            string str = TypeCell.ToString(); var res = str.Split('_');
            str = $"[{LANG.RESOURSES[71]/*древесина*/}: {res[1]}; {LANG.RESOURSES[73]/*глина*/}: {res[2]}; " +
                $"{LANG.RESOURSES[74]/*железо*/}: {res[3]}; {LANG.RESOURSES[72]/*зерно*/}: {res[4]}];";
            return str;
        }

        /// <summary>
        ///     содержит все войска мира Travian. Массив размером 5 элементов:
        ///     0. Римляне. 1. Германцы 2. Галлы 3. Дикая природа 4. Натары<br/>
        ///     Этот класс загружает характеристики из файлов: атака, защита, грузоподъёмность, скорость, стоимость и т.д.
        /// </summary>
        public class TTroops {
            /// <summary> 
            ///     содержит информацию о юните из файлов: "DATA_BASE\IMG\pictograms\unit\" + 4 текстовых файла с названием нации. <br/>
            /// </summary>
            /// <remarks>
            ///     Каждый файл имеет по 20 значимых строк + несколько с комментариями. Каждый юнит имеет по 2 строки: (1. название юнита, 2. характеристики) <br/>
            ///     Класс содержит числа и представлен в виде таблицы разделённой табуляцией. Содержание файла на примере некоторых войск:
            ///     <code>
            ///     медведь   250    140     200       3        20         1000        700       500       1000      760       1:03:00   100       100       100       100       00:11:00 <br/>
            ///     легионер  40     35      50        1        6          50          0         0         0         0         00:00:00  120       100       150       30        00:26:40 <br/>
            ///     т. гром   90     25      40        2        19         75          2200      1900      2040      520       2:34:00   350       450       230       60        0:41:20 <br/>
            ///     вождь     40     60      40        4        4          0           18250     13500     20400     16480     5:23:45   35500     26600     25000     27200     19:35:00 <br/>
            ///     (тип)     атака  защита  защита    потреб.  скорость   грузоподъ-  wood      clay      iron      crop      время     wood      clay      iron      crop      время
            ///     (юнита)          от пех. от конн.  зерна    полей/час  ёмность     исследо-  исследо-  исследо-  исследо-  исследо-  обучение  обучение  обучение  обучение  обучения
            ///     <br/>                                                       ресурсов    вание     вание     вание     вание     вания
            ///     </code>
            /// </remarks>             
            public class TInformation {
                /// <summary> Возвращает или задаёт название военного юнита, а так же вождя и поселенца. </summary>
                public string Name;
                /// <summary> Возвращает или задаёт базовую силу атаки. </summary>
                public uint Attack;
                /// <summary> Возвращает или задаёт базовую силу защиты от пехоты. </summary>
                public uint Defense_vs_Infantry;
                /// <summary> Возвращает или задаёт базовую силу защиты от конницы. </summary>
                public uint Defense_vs_Cavalry;
                /// <summary> Возвращает или задаёт базовое потребление зерна юнитом. </summary>
                public uint Consumption;
                /// <summary> Возвращает или задаёт базовую скорость юнита (полей/час). </summary>
                public uint Speed;
                /// <summary> Возвращает или задаёт базовую грузоподъёмность ресурсов у юнита. </summary>
                public uint LoadCapacity;
                /// <summary> Возвращает или задаёт базовую стоимость исследования юнита в ресурсах в академии. </summary>
                public Res CostOfResearch;
                /// <summary> Возвращает или задаёт базовое время исследования юнита в академии в секундах. </summary>
                public int TimeOfResearch;
                /// <summary> 
                ///     Возвращает или задаёт базовую стоимость обучения юнита в ресурсах в казармах/конюшнях/мастерской/резиденции/дворце. <br/>
                ///     Поскольку в Большой казарме и Большой конюшне стоимость обучения не какая-то своя, а просто в 3 раза выше, то нет нужды для них создавать отдельное поле. Просто при заказе войск в БК умножаем базовую стоимость обучения в обычной казарме и конюшни на 3.
                /// </summary>
                public Res CostOfTraining;
                /// <summary> 
                ///     Возвращает или задаёт базовое время обучения юнита в казармах/конюшнях/мастерской/резиденции/дворце в секундах. <br/>
                ///     Выше упомянутые постройки уникальными параметрами сокращают базовое время обучения юнитов.
                /// </summary>
                public int TimeOfTraining;
            }
            /// <summary> 
            ///     Массив на [10] элементов, содержит информацию о юнитах загруженных из json файлов: "DATA_BASE\JSON\unit_settings\". <br/> 
            ///     Имеет поля: <br/>
            ///     <b> Name: </b>                <inheritdoc cref = "TInformation.Name"> </inheritdoc> <br/>
            ///     <b> Attack: </b>              <inheritdoc cref = "TInformation.Attack"> </inheritdoc> <br/>
            ///     <b> Defense_vs_Infantry: </b> <inheritdoc cref = "TInformation.Defense_vs_Infantry"> </inheritdoc> <br/>
            ///     <b> Defense_vs_Cavalry: </b>  <inheritdoc cref = "TInformation.Defense_vs_Cavalry"> </inheritdoc> <br/>
            ///     <b> Consumption: </b>         <inheritdoc cref = "TInformation.Consumption"> </inheritdoc> <br/>
            ///     <b> Speed: </b>               <inheritdoc cref = "TInformation.Speed"> </inheritdoc> <br/>
            ///     <b> LoadCapacity: </b>        <inheritdoc cref = "TInformation.LoadCapacity"> </inheritdoc> <br/>
            ///     <b> CostOfResearch: </b>      <inheritdoc cref = "TInformation.CostOfResearch"> </inheritdoc> <br/>
            ///     <b> TimeOfResearch: </b>      <inheritdoc cref = "TInformation.TimeOfResearch"> </inheritdoc> <br/>
            ///     <b> CostOfTraining: </b>      <inheritdoc cref = "TInformation.CostOfTraining"> </inheritdoc> <br/>
            ///     <b> TimeOfTraining: </b>      <inheritdoc cref = "TInformation.TimeOfTraining"> </inheritdoc> <br/>
            /// </summary>
            /// <remarks> <inheritdoc cref = "TInformation"> </inheritdoc> </remarks>
            public TInformation[] Information = new TInformation[10];
        }
        /// <summary> 
        ///     Массив войск. Размер массива: [0..4] = 5 шт. В каждом элементе массива хранится <b>Information[0..9]</b> информация о войсках нации. <br/>
        ///     Содержание массива: 0. Римляне. 1. Германцы 2. Галлы 3. Дикая природа 4. Натары <br/>
        ///     Дикая природа и Натары - являются NPC игроками
        /// </summary>
        public TTroops[] Troops = new TTroops[5];

        /// <summary> Метод загружает в Troops данные из файлов json каждого юнита [0..9] <br/> Вызывается один раз при вызове конструктора TGame (при запуске приложения). </summary>
        private void Load_All_Troops() {
            for (int i = 0; i < Troops.Length; i++) {
                Troops[i] = Newtonsoft.Json.JsonConvert.DeserializeObject<TTroops>
                    (File.ReadAllText($"DATA_BASE/JSON/unit_settings/Troops[{i}]_Information[0_9].json"));
            }
        }

        /// <summary> 
        ///     содержит ресурсные + деревенские постройки. В нём хранятся все постройки в количестве 41 шт. <br/>
        ///     Этот класс загружает характеристики из файлов: стоимость, время строительства, потребление, ЕК и т.д.
        /// </summary>
        public class TBuild {
            /// <summary> Возвращает или задаёт номер постройки. <br/><br/> 
            public int ID;
            /// <summary> Возвращает или задаёт название постройки. <br/> Последняя строка из файлов: <b> DATA_BASE/IMG/building/g[ID]-ltr.txt </b> <br/> Поле уже не актуально и служит только для справки. </summary>
            public string Name;

            //Вставка <KeyWord/> позволяет делать отступы от начала строки.
            /// <summary> 
            ///     содержит информацию о постройке из файлов: "DATA_BASE\IMG\building\" + "g0-ltr.txt". Диапазон: g<b>0</b>-ltr - g<b>40</b>-ltr. <br/>
            /// </summary>
            /// <remarks>
            ///     Каждый файл имеет разное количество строк (5-100), содержит числа и представлен в виде таблицы разделённой табуляцией. <br/>
            ///     Количество строк в файле для различых построек: <b> [0..5] </b> - для пекарни, заводов.    <b> [0..20] </b> - для всех построек.    <b> [0..100] </b> - для Чуда Света. <br/>
            ///     Содержание файла на примере <b> БОЛЬШОГО АМБАРА: </b>
            ///     <code>
            ///     <KeyWord/>    20	         43555           54445          38110     	      10890               2      	       24:0:0	         38	           240000 <br/>
            ///     <KeyWord/>    №           лес             глина           железо	      зерно	           потреб.	       время    	  ЕК	           вместимость
            ///     (уровень)   (стоимость)      (стоимость)      (стоимость)      (стоимость)      (потребление зерна) (время)         (производство)   (уникальный параметр 1)
            ///     (постройки) (строительства)  (строительства)  (строительства)  (строительства)  (постройкой на)     (строительства) (единиц)         (бывает 2 параметра)
            ///     <br/>            (постройки)      (постройки)      (постройки)      (постройки)      (этом уровне)       (постройки)     (культуры в час) (в данном случае)
            ///     <br/>            (на этом уровне) (на этом уровне) (на этом уровне) (на этом уровне)                     (в формкте)     (постройкой на)  (у большого амбара)
            ///     <br/>            (в ресурсах)     (в ресурсах)     (в ресурсах)     (в ресурсах)                         (string)        (этом уровне)    (есть вместимость зерна)
            ///     </code>
            /// </remarks>
            public class TInformation {
                /// <summary> Возвращает или задаёт уровень постройки. </summary>
                public uint Level = 0;
                /// <summary> Возвращает или задаёт расходы ресурсов на постройку на каждом её уровене. (Расходы_На_Строительство.<b> wood </b>; Расходы_На_Строительство.<b> crop </b>; и т.д.) </summary>
                public Res Construction_Costs = new Res() { wood = 0, clay = 0, iron = 0, crop = 0, gold = 0 };
                /// <summary> Возвращает или задаёт базовое время строительства постройки в секундах на каждом её уровене. </summary>
                public int Time_Construction = 0;
                /// <summary> Возвращает или задаёт потребление зерна постройкой на каждом её уровне. </summary>
                public int Consumption = 0;
                /// <summary> Возвращает или задаёт производство единиц культуры постройкой на каждом её уровне. </summary>
                public uint ProductionCulture = 0;
                /// <summary> Возвращает или задаёт уникальный параметр постройки на каждом её уровне. </summary>
                /// <remarks> 
                ///     <b> Пример: </b> [вместимость склада/посольства/тайника, скорость обучения/перемещения, 
                ///     грузоподъёмность торговцев, производство ресурсного поля в час и т.д.] <br/>
                ///     Уникальные параметры отсутствуют в файлах txt для: <b> [0] </b> чудо <b> [13] </b> кузница <b> [22] </b> академия <b> [25] </b> резиденция <b> [26] </b> дворец <b> [37] </b> таверна. <br/>
                ///     Два уникальных параметра в файлах txt для: <b> [12] </b> госпиталь <b> [24] </b> ратуша <b> [31..33] стены. </b> <br/>
                ///     Во всех остальных постройках уникальный параметр один. <br/>
                ///     -1 = отсутствие уникального параметра, всё что больше - значит параметр есть. <br/><br/>
                /// </remarks>
                public double Unique_Parameter_1 = -1;
                ///<summary> <inheritdoc cref = "Unique_Parameter_1"> </inheritdoc> </summary>
                ///<remarks> <inheritdoc cref = "Unique_Parameter_1"> </inheritdoc> </remarks>
                public double Unique_Parameter_2 = -1;
            }
            /// <summary> 
            ///     Массив содержит информацию о постройках загруженных из файлов: "DATA_BASE\IMG\building\" + "g0-ltr.txt". Диапазон: g<b>0</b>-ltr - g<b>40</b>-ltr. <br/> 
            ///     Имеет поля: <br/>
            ///     <b> Level: </b>               <inheritdoc cref = "TInformation.Level"> </inheritdoc> <br/>
            ///     <b> Construction_Costs: </b>  <inheritdoc cref = "TInformation.Construction_Costs"> </inheritdoc> <br/>
            ///     <b> Time_Construction: </b>   <inheritdoc cref = "TInformation.Time_Construction"> </inheritdoc> <br/>
            ///     <b> Consumption: </b>         <inheritdoc cref = "TInformation.Consumption"> </inheritdoc> <br/>
            ///     <b> ProductionCulture: </b>   <inheritdoc cref = "TInformation.ProductionCulture"> </inheritdoc> <br/>
            ///     <b> Unique_Parameter_1: </b>  <inheritdoc cref = "TInformation.Unique_Parameter_1"> </inheritdoc> <br/>
            ///     <b> Unique_Parameter_2: </b>  <inheritdoc cref = "TInformation.Unique_Parameter_2"> </inheritdoc> <br/>
            /// </summary>
            /// <remarks> <inheritdoc cref = "TInformation"> </inheritdoc> </remarks>
            public TInformation[] Information = new TInformation[101];
        }
        /// <summary> Массив ресурсных и деревенских построек. Размер массива: [0..41] = 42 шт. </summary>
        public TBuild[] Build = new TBuild[42];

        /// <summary> Метод загружает в Build данные из .json файлов каждой деревенской постройки и ресурсного поля. <br/> Вызывается один раз при вызове конструктора TGame (при запуске приложения). </summary>
        private void Load_All_Buildings() {
            for (int i = 0; i < Build.Length; i++) {
                Build[i] = Newtonsoft.Json.JsonConvert.DeserializeObject<TBuild>
                    (File.ReadAllText($"DATA_BASE/JSON/building_settings/g{i}-ltr.json"));
            }
        }

        /// <summary>
        ///     содержит поля и методы осуществляющие работу мира <b> Travian. </b> <br/>
        ///     Координаты <b>Point(0, 0)</b> зарезервированы для аккаунта <b>Multihunter.</b>
        /// </summary>
        /// <remarks>
        ///     <b>Примечание:</b> <br/>
        ///     После добавления нового поля в класс <b>TMap,</b> выполнить следующее: <br/>
        ///     - добавить поле в метод <b>TMap.SaveMap();</b> с порядковым номером в комменатрии, <br/>
        ///     - добавить поле в метод <b>TMap.LoadMap(...);</b> с тем же порядковым номером в комменатрии. <br/>
        /// </remarks>
        public class TMap {
            /// <summary> 
            ///     Свой генератор случайных чисел у класса <b> TMap </b>. <br/>
            ///     Поле <b> Random </b> проинициализировано через <b> INIT.RandomNext </b>.
            /// </summary>
            public UFO.RANDOM Random = new UFO.RANDOM(UFO.RANDOM.INIT.RandomNext);

            /// <summary>
            ///     Создание экземпляра класса по умолчанию. <br/> default: <b>width</b> = 400; <b>height</b> = 400; <br/>
            /// </summary>
            /// <remarks>
            ///     При размере карты <b>width = 400</b>, <b>height = 400</b> <br/>
            ///     будет создана карта размером <b>801х801</b> с осью <b>(0, 0)</b> в центре. <br/>
            ///     Структура карты: <br/>
            ///     LEFT = [-400..-1];    CENTER = [ось 0];    RIGHT = [1..400] <br/>
            ///     Итоговый размер карты: 400 + 1 + 400 = 801 [0..800, 0..800].
            /// </remarks>
            public TMap() { _TMap(400, 400); }
            /// <summary> Создание экземпляра класса с указанными размерами. <br/> </summary>
            /// <remarks> <inheritdoc cref="TMap(string)"/> </remarks>
            /// <value>
            ///     <b> <paramref name="width"/>: </b> половинчатая <b>ширина</b> карты. <br/>
            ///     <b> <paramref name="height"/>: </b> половинчатая <b>высота</b> карты. <br/>
            /// </value>
            public TMap(int width, int height) { _TMap(width, height); }
            /// <summary> 
            ///     Метод-конструктор. <br/>
            ///     Инициирует некоторые обязательные поля на старте и создаёт массив <b>TCell[][]</b> с указанными размерами. <br/>
            ///     Вызывается только из конструкторов класса. <br/>
            /// </summary>
            /// <value> <inheritdoc cref="TMap(int, int)"/> <br/> </value>
            private void _TMap(int width, int height) {
                Width = width; Height = height;
                Cell = new TCell[width * 2 + 1, height * 2 + 1];
                for (int y = 0; y < Length_Y(); y++) for (int x = 0; x < Length_X(); x++) 
                    Cell[x, y] = new TCell {
                        ID = Cell_ID.Null, TypeResoueces = TypeCell._0_0_0_0, pic_ID = 0, 
                    };
            }

            /// <summary> Хранит информацию о половинчатой ширине карты. </summary>
            private int width = 0;//default [-399..399] Coordinates_World_Travian[0, 0] = Cell[399, 399]
            /// <summary> <inheritdoc cref = "width"/> </summary>
            public int Width { get { return width; } set { width = value; } }
            /// <summary> Хранит информацию о половинчатой высоте карты. </summary>
            private int height = 0;//default [-399..399] Coordinates_World_Travian[0, 0] = Cell[399, 399]
            /// <summary> <inheritdoc cref = "height"/> </summary>
            public int Height { get { return height; } set { height = value; } }
            /// <summary> 
            ///     Хранит координаты (0, 0) на сетке визуальной карты (слева в середине Bitmap-ы pb_Map). <br/>
            ///     Нужны для того чтобы обеспечить сдвиг карты и вывод информации в панель. <br/>
            ///     Координаты хронятся относительно массива <b>Cell[ ][ ].</b> <br/>
            ///     Значение этого поля автоматически обновляется при построении карты в методе <b>Form1.cs.Update_Panels_Map();</b>
            /// </summary>
            private Point location_x0_x0;
            /// <summary> <inheritdoc cref = "location_x0_x0"/> </summary>
            public Point Location_X0_Y0 { get { return location_x0_x0; } set { location_x0_x0 = value; } }
            /// <summary>
            ///     Хранит координаты вокруг которых строится карта. Эти координаты окажутся в центре карты. <br/>
            ///     Осуществляется проверка на выход за границы массива и коррекция значений. <br/>
            ///     Координаты хронятся относительно игровой карты <b>-/+</b> с осью <b>(0, 0)</b> по середине. <br/>
            /// </summary>
            private Point location;
            /// <summary> <inheritdoc cref = "location"/> </summary>
            public Point Location { get { return location; } set { location.X = X_to_Tor(value.X); location.Y = Y_to_Tor(value.Y); } }

            /// <summary> Длина всего двумерного массива карты по оси Х </summary>
            /// <returns> Возвращает ширину двумерного массива. </returns>
            public int Length_X() { return width * 2 + 1; }
            /// <summary> Длина всего двумерного массива карты по оси Y </summary>
            /// <returns> Возвращает высоту двумерного массива. </returns>
            public int Length_Y() { return height * 2 + 1; }
            /// <summary> Площадь всего двумерного массива карты </summary>
            /// <returns> Возвращает площадь двумерного массива. </returns>
            public int SizeMap() { return Length_X() * Length_Y(); }

            /// <summary> Метод замыкает координаты в Тор во все стороны относительно массива <b>Cell[][].</b> </summary>
            /// <returns> Возвращает <b>Point</b> структуру замкнутых координат относительно массива <b>Cell[][].</b> </returns>
            public Point Cell_to_Tor(int X, int Y) {
                if (X < 0) { X *= -1; X %= Length_X(); X = Length_X() - X; } if (X >= Length_X()) { X %= Length_X(); }
                if (Y < 0) { Y *= -1; Y %= Length_Y(); Y = Length_Y() - Y; } if (Y >= Length_Y()) { Y %= Length_Y(); }
                return new Point(X, Y);
            }
            /// <summary> Метод замыкает координату <b>X</b> в Тор относительно оси (0, 0). </summary>
            /// <returns> Возвращает замкнутую координату <b>X</b> относительно деревни аккаунта с учётом оси (0, 0). </returns>
            public int X_to_Tor(int X) {
                if (X < -width) { X *= -1; X %= width; X = width - X + 1; } if (X > width) { X %= width; X = -width + X - 1; } return X;
            }
            /// <summary> Метод замыкает координату <b>Y</b> в Тор относительно оси (0, 0). </summary>
            /// <returns> Возвращает замкнутую координату <b>Y</b> относительно деревни аккаунта с учётом оси (0, 0). </returns>
            public int Y_to_Tor(int Y) {
                if (Y < -height) { Y *= -1; Y %= height; Y = height - Y + 1; } if (Y > height) { Y %= height; Y = -height + Y - 1; } return Y;
            }
            /// <summary> Метод преобразует координаты деревни в системе координат +/- с ценром (0, 0), в систему координат массива Map.Cell[x][y] </summary>
            /// <returns> Возвращает преобразованные координаты. </returns>
            public Point Coord_WorldToMap(Point Коры) { return Cell_to_Tor(Коры.X + Width, Коры.Y + Height); }
            /// <summary> <inheritdoc cref = "Coord_WorldToMap" > </inheritdoc> </summary> 
            /// <returns> <inheritdoc cref = "Coord_WorldToMap" > </inheritdoc> </returns> 
            public Point Coord_WorldToMap(int X, int Y) { return Cell_to_Tor(X + Width, Y + Height); }
            /// <summary> 
            ///     Метод преобразует координаты деревни в системе координат массива Map.Cell[x][y], в систему координат деревни +/- с ценром (0, 0) <br/>
            ///     Нет проверок на кривые координаты.
            /// </summary>
            /// <returns> Возвращает преобразованные координаты. </returns>
            public Point Coord_MapToWorld(int X, int Y) { return new Point(X - Width, Y - Height); }
            /// <summary> <inheritdoc cref = "Coord_MapToWorld" > </inheritdoc> </summary> 
            /// <returns> <inheritdoc cref = "Coord_MapToWorld" > </inheritdoc> </returns> 
            public Point Coord_MapToWorld(Point Коры) { return new Point(Коры.X - Width, Коры.Y - Height); }

            /// <summary> Метод вычисляет кратчайшую дистанцию между двумя ячейками по теореме Пифагора с учётом закольцованного мира. </summary>
            /// <returns> Возвращает кратчайшую дистанцию между двумя точками. </returns>
            public double Dist(Point p1, Point p2) {
                int dx1 = Math.Abs(p1.X - p2.X); int dy1 = Math.Abs(p1.Y - p2.Y);//маршрут 1 внутри массива
                int dx2 = Length_X() - dx1;      int dy2 = Length_Y() - dy1;     //маршрут 2 через границу массива
                int dx, dy; if (dx1 < dx2) dx = dx1; else dx = dx2; if (dy1 < dy2) dy = dy1; else dy = dy2;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            /// <summary> Метод вычисляет коэффициент масштаба карты для отрисовки с учётом текущего разрешения экрана. </summary>
            ///<value> 
            ///     <b>size:</b> размер карты в ячейках. (default = 7x7) <br/>
            ///     <b>FULL_SCREEN_MAP:</b> режим отображения карты. true = увеличенный, false = обычный. (default = false) <br/>
            ///</value> 
            ///<returns> Возвращает коэффициент на который следует умножить размеры текущей карты pb_MAP. </returns>
            public double ScaleMap(int size = 7, bool FULL_SCREEN_MAP = false) {
                return FULL_SCREEN_MAP ? 
                    //полноэкранный режим
                    /*y1080*/UFO.Convert.SCREEN(max:1080) ? 12.8F / size :
                    /*y900*/UFO.Convert.SCREEN(900, 1080) ? 12.1F / size :
                    /*y768*/UFO.Convert.SCREEN(768, 900) ? 11.5F / size :
                    /*y720*/UFO.Convert.SCREEN(min: 768) ? 11.0F / size : 11.0F / size : 
                9.0F / size;//обычный режим
            }

            /// <summary> 
            ///     Сохранение полей класса <b>TMap</b> в файл: <b>map.DAT</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void SaveMap(string path) {
                Directory.CreateDirectory($"{path}/[ map ]/");//создать папку с будущей картой
                using (FileStream fs = new FileStream($"{path}/[ map ]/map.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(Width);               bw.Write(Height);
                        bw.Write(Location_X0_Y0.X);    bw.Write(Location_X0_Y0.Y);
                        bw.Write(location.X);          bw.Write(location.Y);
                }}
            }

            /// <summary>
            ///     Сохранение полей класса <b>TCell</b> в файл: <b>cells.DAT</b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void SaveCells(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                Directory.CreateDirectory($"{path}/[ map ]/");//создать папку с будущей картой
                int SizeCell = Length_X() * Length_Y();

                using (FileStream fs = new FileStream($"{path}/[ map ]/cells.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        for (int y = 0; y < Length_Y(); y++) for (int x = 0; x < Length_X(); x++) {
                            //если войска есть, то длина цепочки в ячейке 43 value значения (3 + 40) = [0..42]
                            //если войск нет, то длина цепочки в ячейке 4 value значения (3 + 1) = [0..3]
                            bw.Write((int)Cell[x, y].ID);
                            bw.Write((int)Cell[x, y].TypeResoueces);
                            bw.Write(Cell[x, y].pic_ID);
                            //МАССИВЫ СОХРАНЯТЬ ПОСЛЕДНИМИ
                            bool b = false; for (int i = 0; i < Cell[x, y].AllTroops.Length; i++) { if (Cell[x, y].AllTroops[i] > 0) { b = true; break; } } 
                            //войска в ячейке есть, записываем всю цепочку
                            if (b) { for (int i=0; i<Cell[x, y].AllTroops.Length; i++) bw.Write(Cell[x,y].AllTroops[i]);
                            } else { bw.Write(-1); } /*войск в ячейке нет, вместо цепочки юнитов храним '-1'*/

                            b = false; for (int i = 0; i < Cell[x, y].VillageTroops.Length; i++) { if (Cell[x, y].VillageTroops[i] > 0) { b = true; break; } } 
                            //войска в ячейке есть, записываем всю цепочку
                            if (b) { for (int i=0; i<Cell[x, y].VillageTroops.Length; i++) bw.Write(Cell[x,y].VillageTroops[i]);
                            } else { bw.Write(-1); } /*войск в ячейке нет, вместо цепочки юнитов храним '-1'*/

                            LoadProcess.LoadText.Text = Text + $" cells: [{(x + (y * Length_X()) + 1)}/{SizeCell}]";
                            LoadProcess._Update();
                        }
                }}
            }

            /// <summary> 
            ///     Метод загружает сохранённую ранее карту из файла: <b>DATA_BASE/ACCOUNTS/[ map ]/cells.DAT</b> в объект <b>Map.</b> <br/>
            ///     Объект <b>Map</b> должен быть предварительно создан, а его размер должен соответствовать количеству ячеек в файле <b>cells.DAT</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            ///     Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b>
            /// </summary>
            /// <remarks>
            ///     <b>Примечание:</b> <br/>
            ///     В загрузке файла <b>map.DAT</b> нет необхродимости т.к. все поля класса <b>TMap</b> получают свои значения во время игры <br/>
            ///     кроме полей: <b>Width / Height</b> которые считываются из файла <b>map.DAT</b> в методе: <b>Form1.cs продолжитьИгруToolStripMenuItem_Click(...); (раздел === Menu Strip ====)</b>
            /// </remarks>
            /// <returns> <b>0:</b> корректное завершение кода, норма. <br/> <b>-1:</b> объект Map не создан. <br/> <b>-2:</b> файл "DATA_BASE/ACCOUNTS/[ map ]/cells.DAT" отсутствует. </returns>
            public int LoadMap(UFO.TLoadProcess LoadProcess, string path) {
                if (this == null) return -1; string Text = LoadProcess.LoadText.Text; int SizeCell = Length_X() * Length_Y();
                if (!File.Exists($"{path}/[ map ]/cells.DAT")) return -2;
                using (FileStream fs = new FileStream($"{path}/[ map ]/cells.DAT", FileMode.Open)) {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                        for (int cell = 0; cell < SizeCell; cell++) { //площадь карты
                            //преобразование номера ячейки в координаты в массива Cell[x][y]
                            int x = cell % Length_X(), y = cell / Length_X();
                            Cell[x, y].ID = (Cell_ID)br.ReadInt32();/*0*/
                            Cell[x, y].TypeResoueces = (TypeCell)br.ReadInt32();
                            Cell[x, y].pic_ID = br.ReadInt32();                 
                            //МАССИВЫ ЗАГРУЖАТЬ ПОСЛЕДНИМИ
                            //войска всегда хранятся в конце сохранённых полей каждой ячейки в файле cells.DAT
                            int value = br.ReadInt32();//читаем величину первого юнита
                            if (value <= -1) { //войск нет
                                for (int i = 0; i < Cell[x, y].AllTroops.Length; i++) Cell[x, y].AllTroops[i] = 0;
                            } else {           //войска есть 
                                Cell[x, y].AllTroops[0] = value;//потому что первого юнита мы уже прочли выше
                                for (int i = 1; i < Cell[x, y].AllTroops.Length; i++)
                                    Cell[x, y].AllTroops[i] = br.ReadInt32();//считываем всю цепочку 
                            }
                            value = br.ReadInt32();//читаем величину первого юнита
                            if (value <= -1) { //войск нет
                                for (int i = 0; i < Cell[x, y].VillageTroops.Length; i++) Cell[x, y].VillageTroops[i] = 0;
                            } else {           //войска есть 
                                Cell[x, y].VillageTroops[0] = value;//потому что первого юнита мы уже прочли выше
                                for (int i = 1; i < Cell[x, y].VillageTroops.Length; i++)
                                    Cell[x, y].VillageTroops[i] = br.ReadInt32();//считываем всю цепочку
                            }
                            LoadProcess.LoadText.Text = Text + $" cells: [{cell + 1}/{SizeCell}]";
                            LoadProcess._Update();
                        }
                }}
                return 0;
            }

            /// <summary> 
            ///     Метод создаёт случайную карту с указанными размерами. <br/>
            ///     Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b>
            /// </summary>
            /// <remarks>
            ///     При размере карты <b>width = 400</b>, <b>height = 400</b> <br/>
            ///     будет создана карта размером <b>801х801</b> с осью <b>(0, 0)</b> в центре. <br/>
            ///     <b>Cell[x, y] = [-400..400, -400..400]</b> <br/>
            ///     <b>Coordinates_World_Travian[0, 0] = Cell[400, 400]</b>
            /// </remarks>
            public void CreateMap(UFO.TLoadProcess LoadProcess) {
                string Text = LoadProcess.LoadText.Text; int SizeCell = Length_X() * Length_Y();
                for (int y = 0; y < Length_Y(); y++) for (int x = 0; x < Length_X(); x++) {
                    Cell[x, y].Location = new Point(x, y);
                    int NCell = x + y * Length_X();
                    if (Cell[x, y].ID != Cell_ID.Null) {
                        //поля этой ячейки уже отредактированы в методе загрузки деревни
                        //там вызывается метод: addAccountOnMap(...) который делает это, по этому пропускаем ячейку
                        LoadProcess.LoadText.Text = Text + $" cells: [{NCell + 1}/{SizeCell}]"; LoadProcess._Update();
                        continue;//ячейка занята деревней игрока или бота
                    }

                    Cell[x, y].TypeResoueces = TypeCell._0_0_0_0; Cell[x, y].pic_ID = 0;
                    Cell[x, y].LinkVillage = null; Cell[x, y].LinkAccount = null;
                    //всех оазисов в сумме примерно на 15% от размера карты +/- !!!!!!!!!!!!!
                    //частота генерации ячеек
                    if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_wood25;               
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_wood50;        
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_wood25_crop25; 
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_clay25; Cell[x, y].pic_ID = Random.RND(0, 1); 
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_clay50;        
                    } else if (Random.RND(1, 100) <= 2/* % */) { Cell[x, y].ID = Cell_ID.Oasis_clay25_crop25; 
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_iron25;        
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Oasis_iron50; Cell[x, y].pic_ID = Random.RND(0, 1); 
                    } else if (Random.RND(1, 100) <= 2/* % */) { Cell[x, y].ID = Cell_ID.Oasis_iron25_crop25; 
                    } else if (Random.RND(1, 100) <= 2/* % */) { Cell[x, y].ID = Cell_ID.Oasis_crop25; Cell[x, y].pic_ID = Random.RND(0, 1); 
                    } else if (Random.RND(1, 100) <= 2/* % */) { Cell[x, y].ID = Cell_ID.Oasis_crop50;        
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Water; 
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Mountains; 
                    } else if (Random.RND(1, 100) <= 1/* % */) { Cell[x, y].ID = Cell_ID.Forest; 
                    } 
                    else { //дикое поле
                        TypeCell type_cell;
                        if (Random.RND(1, 100) <= 4/* % */) type_cell = TypeCell._1_1_1_15;
                        else if (Random.RND(1, 100) <= 6/* % */) type_cell = TypeCell._3_3_3_9;
                        else if (Random.RND(1, 100) <= 8/* % */) type_cell = TypeCell._4_3_4_7;
                        else if (Random.RND(1, 100) <= 8/* % */) type_cell = TypeCell._4_4_3_7;
                        else if (Random.RND(1, 100) <= 8/* % */) type_cell = TypeCell._3_4_4_7;
                        else if (Random.RND(1, 100) <= 5/* % */) type_cell = TypeCell._3_4_5_6;
                        else if (Random.RND(1, 100) <= 5/* % */) type_cell = TypeCell._4_3_5_6;
                        else if (Random.RND(1, 100) <= 5/* % */) type_cell = TypeCell._5_4_3_6;
                        else if (Random.RND(1, 100) <= 5/* % */) type_cell = TypeCell._3_5_4_6;
                        else type_cell = TypeCell._4_4_4_6;
                        Cell[x, y].ID = Cell_ID.Wild_Field;   Cell[x, y].TypeResoueces = type_cell;
                        Cell[x, y].pic_ID = Random.RND(0, 7);//имеется 8 картинок данного типа
                    }

                    //ДИКАЯ ПРИРОДА. чем сильнее животное, тем меньше его в оазисе
                    if (Cell[x, y].TypeResoueces == TypeCell._0_0_0_0) { 
                        //0.Крыса   1.Паук   2.Змея   3.Летучая_Мышь   4.Кабан   5.Волк   6.Медведь  7.Крокодил  8.Тигр  9.Слон
                        float MIN = 1F, MAX = 1F; for (int i = 0; i < Cell[x, y].AllTroops.Length; i++) {
                            int R; if (i >= 30 && i <= 39) {//генерируем кол-во войск у природы
                                R = Random.RND((int)(-100 * MIN), (int)(100 * MAX)); if (R < 0) R = 0; MIN += 0.1F; MAX -= 0.1F;
                            } else R = 0;//остальные типы войск
                            Cell[x, y].AllTroops[i] = R;//у природы рисуем войска, в остальных [] нули
                        }
                    }
                    //ячейка обработана
                    LoadProcess.LoadText.Text = Text + $" cells: [{NCell + 1}/{SizeCell}]"; LoadProcess._Update();
                }
            }

            /// <summary> Метод принимает на вход текущее население деревни, проверяет актуальный размер картинки деревни на карте. <br/> Если переданное население не изменило картинку на карте, то она останется прежней. </summary>
            /// <value> <b> Village_Population: </b> текущее население деревни. </value>
            /// <returns> Возвращает актуальный Cell.ID соответствующий текущему населению деревни. <br/> В случае поломки логики метода, <b> return </b> вернёт 0 (null). </returns>
            public Cell_ID Refresh_ID(int Village_Population) {
                //Cell_ID Cell_ID = Cell_ID.Null;//default
                return Village_Population >= (int)SizeVillage.Large ? Cell_ID.Village_Large ://[900..MAX] 
                    Village_Population >= (int)SizeVillage.Medium ? Cell_ID.Village_Medium ://[600..899]
                    Village_Population >= (int)SizeVillage.Small ? Cell_ID.Village_Small ://[300..599]
                    Village_Population >= (int)SizeVillage.Tiny ? Cell_ID.Village_Tiny ://[0..299]
                Cell_ID.Null;//default
            }

            /// <summary> Метод добавляет деревню на карту и помечает принадлежащие ей оазисы. Вызывается перед генерацией карты! </summary>
            /// <value>
            ///     <b>Account:</b> аккаунт добавляемой деревни. <br/>
            ///     <b>Village:</b> добавляемая деревня. <br/>
            ///     <b>Player:</b> Player (игрок, аккаунт bot[0]) (единственный TPlayer у которого хранятся свойства войск, деревень, стек событий и т.д.). <br/>
            /// </value>
            public void addAccountOnMap(TPlayer Account, TVillage Village, TGame Game) {
                int x = Width + Village.Coordinates_World_Travian.X; int y = Height + Village.Coordinates_World_Travian.Y;
                Cell[x, y].ID = Refresh_ID(Village.Population);
                Cell[x, y].pic_ID = 0;             Cell[x, y].TypeResoueces = Village.Type_Resources;
                Cell[x, y].LinkAccount = Account;  Cell[x, y].LinkVillage = Village;
                for (int i = 0; i < Village.OasisList.Count; i++) {
                    Cell[Village.OasisList[i].X, Village.OasisList[i].Y].LinkAccount = Account;
                    Cell[Village.OasisList[i].X, Village.OasisList[i].Y].LinkVillage = Village;
                }

                //ТЕСТ - генерация случайной численности армии аккаунта в деревне и вычисление потребления зерна
                int m = (int)Account.Folk_Name * Game.Troops[0].Information.Length;/*Lenght = 10 шт*/
                for (int i = 0; i < Cell[x, y].AllTroops.Length - 1; i++) {
                    int CountTroops = 0;
                    if (i - m >= 0 && i < m + Cell[x, y].VillageTroops.Length) {
                        CountTroops = Random.RND(-150, 100); if (CountTroops < 0) CountTroops = 0;
                        Cell[x, y].VillageTroops[i - m] = CountTroops;//войска аккаунта
                    }
                    Cell[x, y].AllTroops[i] = CountTroops;//общие войска
                    //вычисление суммарного потребления зерна
                    Village.Crop_Consumption += ((int)Game.Troops[i / 10].Information[i % 10].Consumption * CountTroops);
                }
                //тест размещения войск
                //for (int i = 0; i < Cell[x, y].ValueAllTroops.Length; i++) Cell[x, y].ValueAllTroops[i] = i;
            }

            /// <summary> содержит поля и методы для работы с картой мира <b> Travian. </b> </summary>
            /// <remarks>
            ///     <b>Примечание:</b> <br/>
            ///     После добавления нового поля в класс <b>TMap,</b> выполнить следующее: <br/>
            ///     - добавить поле в метод <b>TMap.SaveMap();</b> с порядковым номером в комменатрии, <br/>
            ///     - добавить поле в метод <b>TMap.LoadMap(...);</b> с тем же порядковым номером в комменатрии. <br/>
            /// </remarks>
            public class TCell {
                /// <summary> Хранит путь файла для этой ячейки в который сохраненятся все поля класса <b>TCell</b>. </summary>
                public string Path = "";
                /// <summary> Хранит имя файла для этой ячейки в которое сохраненятся все поля класса <b>TCell</b>. </summary>
                public string FileName = "";

                /// <summary> Хранит информацию о типе ячейки. </summary>
                private Cell_ID id = Cell_ID.Null;
                /// <summary> <inheritdoc cref = "id"/> </summary>
                public Cell_ID ID { get { return id; } set { id = value; } }
                /// <summary> Хранит информацию о типе полей ячейки. </summary>
                private TypeCell typeresoueces;
                /// <summary> <inheritdoc cref = "typeresoueces"/> </summary>
                public TypeCell TypeResoueces { get { return typeresoueces; } set { typeresoueces = value; } }
                /// <summary>
                ///     Хранит номер загружаемой картинки из папки <b>DATA_BASE/IMG/map/Cells/</b> для <b>ID.</b> <br/>
                ///     Если картинка всего одна, тогда <b>pic_ID = -1.</b>
                /// </summary>
                private int pic_id = 0;
                /// <summary> <inheritdoc cref = "pic_id"/> </summary>
                public int pic_ID { get { return pic_id; } set { pic_id = value; } }
                /// <summary> 
                ///     Хранит сумму всех войск в конкретной ячейке массива <b>Cell[][]</b> + собственные войска <b>ValueVillageTroops[]</b>. <br/>
                ///     На данный момент массив имеет размер 51 элемент [0..50]. <br/>
                ///     [0..9] = Римляне / Romans <br/> [10..19] = Германцы / Germans <br/>
                ///     [20..29] = Галлы / Gauls <br/> [30..39] = Дикая природа / Nature <br/> [40..49] = Натары / Natars <br/>
                ///     [50] = Герои / Heroes (только подкреп) <br/>
                /// </summary>
                public int[] AllTroops = new int[51];
                /// <summary>
                /// Хранит сумму войск деревни принадлежащих конкретной ячейке массива <b>Cell[][].</b> <br/>
                /// <b>ValueVillageTroops[10]</b> частный случай от <b>ValueAllTroops[51].</b>
                /// </summary>
                public int[] VillageTroops = new int[10];
                /// <summary> 
                ///     Хранит ссылку на деревню в этой ячейке. <br/>
                ///     Зацепка ссылки происходит в методе <b>TMap.addAccountOnMap();</b>
                /// </summary>
                public TVillage LinkVillage = null;
                /// <summary>
                ///     Хрранит ссылку на аккаунт в этой ячейке. <br/>
                ///     Зацепка ссылки происходит в методе <b>TMap.addAccountOnMap();</b>
                /// </summary>
                public TPlayer LinkAccount = null;
                /// <summary> Хранит координаты Х/Y каждой ячейки в системе координат Cell[x, y]; </summary>
                public Point Location;

                /// <summary> Метод проверяет ссылки ячейки: <b>LinkAccount/LinkVillage</b>. </summary>
                /// <returns> Возвращает <b>true</b> если этой ячейкой владеет аккаунт и <b>false</b>, если нет. </returns>
                public bool IsFree() { return LinkAccount == null && LinkVillage == null; }
                /// <summary> Метод проверяет <b>ID</b> поле на принадлежность к оазису. </summary>
                /// <returns> Возвращает <b>true</b> если ячейка является оазисом и <b>false</b>, если нет. </returns>
                public bool IsOasis() { return ID >= Cell_ID.Oasis_wood25 && ID <= Cell_ID.Oasis_crop50; }
                /// <summary>
                /// Метод конвертирует тип ячейки по его <b>ID</b> полю в эквивалент строки в текущем выбранном языке LANGUAGES.RESOURSES[]; </summary>
                /// <value> <b><paramref name="RES"/>:</b> массив из объекта LANGUAGES.RESOURSES[]; </value>
                /// <returns> Возвращает строковой эквивалент типа Cell.ID <b>enum Cell_ID</b> в выбранном языке. Если значение вне диапазона, метод возвращает строку: $"[Error. Invalid ID] Cell[,].ID = {ID}". </returns>
                public string ID_ToText(string[] RES) {
                    string tmp_village = $"{RES[18].Substring(0, 1).ToLower()}{RES[18].Substring(1, RES[18].Length - 1)}";/*деревня*/
                    return ID == Cell_ID.Null ? "Null" : ID == Cell_ID.Wild_Field ? RES[14]/*Покинутая долина*/
                      : ID == Cell_ID.Water ? RES[131]/*Озеро*/ : ID == Cell_ID.Forest ?
                          RES[134]/*Лес*/ : ID == Cell_ID.Mountains ? RES[133]/*Горы*/
                              : ID == Cell_ID.Village_Tiny ? $"{RES[145]/*Крошечная*/} {tmp_village}"
                                  : ID == Cell_ID.Village_Small ? $"{RES[146]/*Маленькая*/} {tmp_village}"
                                      : ID == Cell_ID.Village_Medium ? $"{RES[147]/*Средняя*/} {tmp_village}"
                                          : ID == Cell_ID.Village_Large ? $"{RES[148]/*Большая*/} {tmp_village}"
                                              : ID >= Cell_ID.Oasis_wood25 && ID <= Cell_ID.Oasis_crop50 ?
                                                  RES[9]/*Оазис*/ : $"[Error. Invalid ID] Cell[,].ID = {ID}";
                }
            }

            /// <summary> Двумерный массив карты Травиан. Содержит свойства в каждой ячейке. </summary>
            public TCell[,] Cell = null;
        }

        /// <summary>
        ///     Экземпляр класса "Карта". Всё что нужно для карты мира <b>Travian.</b> <br/>
        ///     Размер карты хранится в классе <b>Global.</b>
        /// </summary>
        public TMap Map = null;



        /// <summary>
        ///     static class FILE. <br/> Содержит static методы для работы с файлами. Создание экземпляра класса не требуется. <br/> <br/>
        ///     <b> CreateFile() </b> -                 <inheritdoc cref = "CreateFile"> </inheritdoc> <br/><br/>
        ///     <b> LengthFile() </b> -                 <inheritdoc cref = "LengthFile"> </inheritdoc> <br/><br/>
        ///     <b> LoadAllLines() </b> -               <inheritdoc cref = "LoadAllLines"> </inheritdoc> <br/><br/>
        ///     <b> LoadAllKeys() </b> -                <inheritdoc cref = "LoadAllKeys"> </inheritdoc> <br/><br/>
        ///     <b> LoadAllValues() </b> -              <inheritdoc cref = "LoadAllValues"> </inheritdoc> <br/><br/>
        ///     <b> LoadAllLines_Or_CreateFile() </b> - <inheritdoc cref = "LoadAllLines_Or_CreateFile"> </inheritdoc> <br/><br/>
        ///     <b> LoadLine() </b> -                   <inheritdoc cref = "LoadLine"> </inheritdoc> <br/><br/>
        ///     <b> SaveLine() </b> -                   <inheritdoc cref = "SaveLine"> </inheritdoc> <br/><br/>
        ///     <b> LoadValue() </b> -                  <inheritdoc cref = "LoadValue"> </inheritdoc> <br/><br/>
        ///     <b> LoadKey() </b> -                    <inheritdoc cref = "LoadKey"> </inheritdoc> <br/><br/>
        /// </summary>
        public static class FILE {
            /// <summary>
            ///     Метод создаёт массив строк троеточий "..." до <b> length </b> включительно и сохраняет их в файл. <br/>
            ///     Если файла нет, он будет создан. В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <remarks> 
            ///     Замещение троеточий на реальные данные идёт в разброс, по этому метод заранее определяет кол-во строк в файле.
            /// </remarks>
            /// <value>
            ///     <b> length: </b> длина строк в файле. <br/>
            ///     <b> Path: </b> путь к файлу без имени самого файла. <br/>
            ///     <b> FileName: </b> имя файла. <br/>
            ///     <b> tmp: </b> массив строк "..." размером length. <br/>
            /// </value>
            /// <returns> 
            ///     Если код отработал успешно, переданная по ссылке переменная <b> tmp[] </b> будет хранить текст файла.
            /// </returns>
            public static void CreateFile(int length, string Path, string FileName, ref string[] tmp) {
                while (true) {
                    tmp = new string[length + 1]; for (int i = 0; i < length + 1; i++) { tmp[i] = "..."; }
                    try { File.WriteAllLines(Path + FileName, tmp, Encoding.Default); break; }
                    catch (DirectoryNotFoundException) { Directory.CreateDirectory(Path); }//создать папку с названием ник нэйма
                }
            }

            /// <summary> Метод открывает файл по указанному пути и возвращает количество строк в файле в формате <b> int </b>. </summary>
            /// <remarks> В случае неудачи осуществляется выход из приложения. </remarks>
            /// <value> <inheritdoc cref = "LoadAllLines" select = "value"> </inheritdoc> </value>
            /// <returns> Длина файла в формате <b> int </b>. <br/> </returns>
            public static int LengthFile(string Path, string FileName) {
                string[] tmp = new string[1];
                try { tmp = File.ReadAllLines(Path + FileName, Encoding.Default); }
                catch (DirectoryNotFoundException) {
                    MessageBox.Show($"Error 4.\nОшибка чтения. Дериктория '{Path}' не найдена.\nFILE.Count_Strings(...);\nВыход из программы.");
                    Environment.Exit(1);
                }
                catch (FileNotFoundException) {
                    MessageBox.Show($"Error 5.\nОшибка чтения. Файл '{Path}{FileName}' не найден.\nFILE.Count_Strings(...);\nВыход из программы.");
                    Environment.Exit(1);
                }
                return tmp.Length;
            }

            /// <summary> 
            ///     Метод пытается прочитать все строки из файла в массив <b> string[] tmp </b>. <br/>
            ///     В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <value>
            ///     <b> Path: </b> путь к файлу без имени самого файла. <br/>
            ///     <b> FileName: </b> имя файла.
            /// </value>
            /// <returns> Весь массив строк в формате <b> string[] </b>. <br/> </returns>
            public static string[] LoadAllLines(string Path, string FileName) {
                string[] tmp = new string[1];
                try { tmp = File.ReadAllLines(Path + FileName, Encoding.Default); }
                catch (DirectoryNotFoundException) {
                    MessageBox.Show($"Error 6.\nОшибка чтения. Дериктория '{Path}' не найдена.\nFILE.LoadAllLines(...);\nВыход из программы.");
                    Environment.Exit(1);
                }
                catch (FileNotFoundException) {
                    MessageBox.Show($"Error 7.\nОшибка чтения. Файл '{Path}{FileName}' не найден.\nFILE.LoadAllLines(...);\nВыход из программы.");
                    Environment.Exit(1);
                }
                return tmp;
            }

            /// <summary> 
            ///     Метод пытается прочитать все <b>Key</b> значения в каждой строке файла в массив <b> string[] tmp </b>. <br/>
            ///     В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <remarks> Метод работает с файлами имеющими структуру: <b> "key = value" </b>. </remarks>
            /// <value> <inheritdoc cref = "LoadAllLines" select = "value"> </inheritdoc> </value>
            /// <returns> Весь массив строк со значениями <b>Key</b> в формате <b> string[] </b>. <br/> </returns>
            public static string[] LoadAllKeys(string Path, string FileName) {
                string[] tmp = LoadAllLines(Path, FileName); string key;
                for (int i = 0; i < tmp.Length; i++) { key = "";
                    for (int j = 0; j < tmp[i].Length; j++) { if (tmp[i][j] == '=') break; key += tmp[i][j]; }
                    tmp[i] = key;
                }
                return tmp;
            }

            /// <summary> 
            ///     Метод пытается прочитать все <b>Value</b> значения в каждой строке файла в массив <b> string[] tmp </b>. <br/>
            ///     В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <remarks> Метод работает с файлами имеющими структуру: <b> "key = value" </b>. </remarks>
            /// <value> <inheritdoc cref = "LoadAllLines" select = "value"> </inheritdoc> </value>
            /// <returns> Весь массив строк со значениями <b>Value</b> в формате <b> string[] </b>. <br/> </returns>
            public static string[] LoadAllValues(string Path, string FileName) {
                string[] tmp = LoadAllLines(Path, FileName); string value;
                for (int i = 0; i < tmp.Length; i++) { value = "";
                    for (int j = 0; j < tmp[i].Length; j++) { if (tmp[i][j] == '=') { j++;
                            for (; j < tmp[i].Length; j++) value += tmp[i][j]; break;
                    }} tmp[i] = value;
                }
                return tmp;
            }


            /// <summary>
            ///     Метод пытается прочитать все строки из файла в массив <b> string[] tmp </b>. <br/>
            ///     В случае неудачи <inheritdoc cref = "CreateFile" select = "summary"> </inheritdoc> <br/>
            /// </summary>
            /// <remarks>
            ///     Параметр <b> length </b> используется в случае неудачи чтения файла.
            /// </remarks>
            /// <value> 
            ///     <b> length: </b> длина строк в файле. <br/>
            ///     <inheritdoc cref = "LoadAllLines" select = "value"> </inheritdoc> <br/>
            ///     <b> tmp: </b> массив строк "..." размером length.
            /// </value>
            /// <returns> <inheritdoc cref = "CreateFile" select = "returns"> </inheritdoc> </returns>
            public static void LoadAllLines_Or_CreateFile(int length, string Path, string FileName, ref string[] tmp) {
                try { tmp = File.ReadAllLines(Path + FileName, Encoding.Default); }
                catch (DirectoryNotFoundException) { CreateFile(length, Path, FileName, ref tmp); }//директория не найдена
                catch (FileNotFoundException) { CreateFile(length, Path, FileName, ref tmp); }//файл не найден
            }

            /// <summary>
            ///     Метод открывает файл по указанному пути и считывает из него всю строку с номером строки <b> index </b> в формате <b> string </b>. <br/>
            ///     В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <remarks> <inheritdoc cref = "LoadAllLines"> </inheritdoc> </remarks>
            /// <value>
            ///     <b> index: </b> номер строки в файле. <br/>
            ///     <inheritdoc cref = "LoadAllLines" select = "value"> </inheritdoc>
            /// </value>
            /// <returns> Вся строка в формате <b> string </b>. <br/> </returns>
            public static string LoadLine(int index, string Path, string FileName) {
                string[] tmp = new string[1];
                LoadAllLines_Or_CreateFile(index, Path, FileName, ref tmp);
                if (index < 0 || index >= tmp.Length)
                { MessageBox.Show($"Error 8.\nОшибка в FILE.LoadLine()\nПуть файла: '{Path}{FileName}'." +
                    "index за границей массива.\nВыход из программы."); Environment.Exit(1); return ">_<"; }
                return tmp[index];
            }

            /// <summary>
            ///     Метод открывает файл по указанному пути и записывает в него строку <b> Line </b> с номером строки <b> index </b> в формате <b> string </b>. <br/>
            ///     Если файла нет, он будет создан. В случае неудачи осуществляется выход из приложения.
            /// </summary>
            /// <remarks> <inheritdoc cref = "LoadLine" select = "remarks"> </inheritdoc> </remarks>
            /// <value>
            ///     <inheritdoc cref = "LoadLine" select = "value"> </inheritdoc> <br/>
            ///     <b> Line: </b> строка которую необходимо записать в файл (представленна в виде <b> "var = value" </b>).
            /// </value>
            /// <returns>
            ///     <b> true - </b> если строка успешно записана в файл. <br/>
            ///     <b> false - </b> не возвращается по причине закрытия приложения. <br/>
            /// </returns>
            public static bool SaveLine(int index, string Path, string FileName, string Line) {
                if (index < 0) { MessageBox.Show($"Error 9.\nОшибка в FILE.SaveValue()\nПуть файла: '{Path}{FileName}'." +
                    "index < 0.\nВыход из программы."); Environment.Exit(1); return false; }
                string[] tmp = new string[1];
                //если файл не существует, создаём его и вставляем пробелы на каждой строке до index включительно
                LoadAllLines_Or_CreateFile(index, Path, FileName, ref tmp);
                //если файл пустой или не полный, вставляем пробелы на каждой строке до index включительно
                if (tmp.Length == 0) { CreateFile(index, Path, FileName, ref tmp); }
                else if (index >= tmp.Length) {
                    string[] tmp2 = new string[index + 1];
                    for (int i = 0; i < index + 1; i++) if (i < tmp.Length) tmp2[i] = tmp[i]; else tmp2[i] = "...";
                    tmp = tmp2;
                }
                tmp[index] = Line;
                //int j = 0;
                while (true) { //попытка записи. почемуто иногда строка записывается с N-ной попытки.
                    try { File.WriteAllLines(Path + FileName, tmp, Encoding.Default); break; }
                    catch (IOException) { /*MessageBox.Show("Попытка №" + j + "\nНе получилось записать строку: " + Line + ".\nПробуем ещё.");*/ }
                    //j++;
                }
                return true;
            }

            /// <summary>
            ///     Метод открывает файл по указанному пути и считывает из него <b> value </b> значение с номером строки <b> index </b> в формате <b> string </b>.
            /// </summary>
            /// <remarks> Метод работает с файлами имеющими структуру: <b> "key = value" </b>. </remarks>
            /// <value> <inheritdoc cref = "LoadLine" select = "value"> </inheritdoc> </value>
            /// <returns> <b> Value </b> значение в формате <b> string </b>. </returns>
            public static string LoadValue(int index, string Path, string FileName) {
                string tmp = LoadLine(index, Path, FileName);
                string value = "";
                for (int i = 0; i < tmp.Length; i++) if (tmp[i] == '=') { i++;
                        for (; i < tmp.Length; i++) value += tmp[i]; break; }
                return value;
            }

            /// <summary>
            ///     Метод открывает файл по указанному пути и считывает из него <b> key </b> значение с номером строки <b> index </b> в формате <b> string </b>.
            /// </summary>
            /// <remarks> Метод работает с файлами имеющими структуру: <b> "key = value" </b>. </remarks>
            /// <value> <inheritdoc cref = "LoadLine" select = "value"> </inheritdoc> </value>
            /// <returns> <b> Key </b> значение в формате <b> string </b>. </returns>
            public static string LoadKey(int index, string Path, string FileName) {
                string tmp = LoadLine(index, Path, FileName); string key = "";
                for (int i = 0; i < tmp.Length; i++) { if (tmp[i] == '=') break; key += tmp[i]; }
                return key;
            }

        }

    }
}
