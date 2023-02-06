using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Globalization;
using static GameLogica.Enums_and_structs;
using System.Windows.Forms;

namespace GameLogica {
public partial class TGame {
        /// <summary> содержит в себе всю не визуальную часть игры кроме логики. <br/><br/>
        ///     <b> Подклассы TPlayer: </b> <br/>
        ///     class <b> TGame.TPlayer.TVillage: </b> подкласс <inheritdoc cref="TVillage"/> <br/>
        ///     class <b> TGame.TPlayer.TVillage.TInfo: </b> подкласс <inheritdoc cref="TVillage.TSlot"/> <br/>
        /// </summary>
        public partial class TPlayer {
            private NumberFormatInfo provider = new NumberFormatInfo();//для конвертации Double.toString чисел с точками

            /// <summary> 
            ///     Свой генератор случайных чисел у класса <b> Player </b>. <br/>
            ///     Поле <b> Random </b> проинициализировано через <b> INIT.RandomNext </b>.
            /// </summary>
            public UFO.RANDOM Random = new UFO.RANDOM(UFO.RANDOM.INIT.RandomNext);


            /// <summary> содержит стартовые свойства героя загруженные из json файлов и текущие. Поля объекта изменяемые в процессе игры. </summary>
            /// <remarks>
            ///     При добавлении нового поля в этом классе, сделать следующее: <br/>
            ///     - в папке <b>"DATA_BASE/json/unit_settings"</b> в файлах наций <b>"Hero...json"</b> добавить строку с названием поля и значением в нужном месте <br/>
            ///     - в методе <b>LoadHero</b> в инициализации <b>Hero</b> добавить созданное поле <br/>
            ///     - в методе <b>SaveHero()</b> добавить созданное поле <br/>
            /// </remarks>
            public class THero {
                //все поля загружаются из файла Hero.DAT
                /// <summary> Название деревни в которой прописан герой. <br/> При переименовании деревни, в которой прописан герой, следует так же переименовать и это поле. <br/> прописка героя создаётся или меняется в момент создания героя и перепрописке его в другой деревне. </summary>
                public string Registration_Village = "";
                //поля json файла
                /// <summary> Возвращает или задаёт имя героя. </summary>
                public string Name = "";
                /// <summary> Возвращает или задаёт флаг, сигнализирующий о том, создан ли герой. <br/> <b>true</b> = герой создан, <b>false</b> = герой не создан. </summary>
                public bool isCreated = false;
                /// <summary> Возвращает или задаёт уровень героя. </summary>
                public int Level = 0;
                /// <summary> Возвращает или задаёт количество убитых юнитов героем включая природу и натар. </summary>
                public int Kills = 0;
                /// <summary> Возвращает или задаёт количество доступных очков распределения у героя. </summary>
                public int Points = 0;
                /// <summary> Возвращает или задаёт количество очков распределения за уровень героя. </summary>
                public int Points_per_lvl = 0;
                /// <summary> Возвращает или задаёт очки здоровья героя. <br/> Убитого героя можно восстановить в таверне. </summary>
                public int HP = 0;
                /// <summary> Возвращает или задаёт очки регенерации здоровья героя в сутки. </summary>
                public int HP_Regeneration = 0;
                /// <summary> Возвращает или задаёт индивидуальную силу героя, которая влияет на его атаку и оборону. <br/> Герой является кавалерией, если Horse = true и пехотинцем, если Horse = false. </summary>
                public int Power = 0;
                /// <summary> Возвращает или задаёт величину на которую увеличивается сила героя при распределении очков опыта. <br/> 1 очко распределения = этому значению. </summary>
                public int Power_Add = 0;
                /// <summary> Возвращает или задаёт бонус атаки героя, который увеличивает силу атаки всех войск, находящихся с героем. <br/> Бонус является процентным. </summary>
                public double Bonus_Attack = 0;
                /// <summary> Возвращает или задаёт величину на которую увеличивается бонус атаки героя при распределении очков опыта. <br/> 1 очко распределения = этому значению. </summary>
                public double Bonus_Attack_Add = 0;
                /// <summary> Возвращает или задаёт бонус защиты героя, который увеличивает силу защиты всех собственных войск, <br/> находящихся с героем вне зависимости, откуда они пришли. <br/> Бонус является процентным. </summary>
                public double Bonus_Defense = 0;
                /// <summary> Возвращает или задаёт величину на которую увеличивается бонус защиты героя при распределении очков опыта. <br/> 1 очко распределения = этому значению. </summary>
                public double Bonus_Defense_Add = 0;
                /// <summary> Возвращает или задаёт бонус увеличения добычи ресурсов в деревне в которой прописан герой. <br/> Бонус переключаемый. В единицу времени можно нарастить выработку только одного ресурса в час. <br/> Бонус является процентным. </summary>
                public Res Bonus_Resourses;
                /// <summary> Возвращает или задаёт величину на которую увеличивается бонус увеличения добычи ресурсов в деревне в которой находится герой. <br/> поле увеличивает множители бонуса добычи ресурсов, либо на эту величину к одному ресурсу, либо на (Bonus_Resourses_Add / 5) к каждому ресурсу. </summary>
                public int Bonus_Resourses_Add = 0;
                /// <summary> Возвращает или задаёт бонус фарма героя. <br/> Бонус является процентным и сокращает вместимость вражеских тайников если в атаке участвует герой. </summary>
                public int Bonus_Farm = 0;
                /// <summary> Возвращает или задаёт бонус сокращения времени восстановления героя. <br/> После смерти героя, в поле TimerOfRecovery назначается время восстановления в секундах и дополнительно вычитается Bonus_Recovery_Time; <br/> Время является дробной величиной. </summary>
                public double Bonus_Recovery_Time = 0;
                /// <summary> Возвращает или задаёт величину на которую увеличивается бонус сокращения времени восстановления героя. <br/> Время является дробной величиной. </summary>
                public double Bonus_Recovery_Time_Add = 0;
                /// <summary> Возвращает или задаёт текущее потребление зерна в час героем. <br/> Созданный герой в таверне наследует потребление юнита из которого он был создан. </summary>
                public int Consumption = 0;
                /// <summary> Возвращает или задаёт текущую скорость героя: полей/в час. <br/> Созданный герой в таверне наследует скорость юнита из которого он был создан. </summary>
                public int Speed = 0;
                /// <summary> Возвращает или задаёт наличие коня у героя. По умолчанию герой пеший. <br/> true - герой верхом, false - герой пеший. </summary>
                public bool Horse = false;
                /// <summary> Возвращает или задаёт величину на которую увеличивается текущая скорость героя если он сидит верхом на лошади. <br/> Добавление к текущей скорости передвижения героя на эту величину происходит в момент изменения значения поля Horse на true. <br/> Скорость героя на войска не распространяется. </summary>
                public int Speed_Horse_Add = 0;
                /// <summary> Возвращает или задаёт величину таймера восстановления героя на каждом уровне в секундах. <br/> Поле является таймером. Пока герой жив, TimeOfResurrection = 0, как только HP = 0, в это поле помещается кол-во секунд которое должно пройти до полного восстановления героя на его уровне. </summary>
                public int TimerOfRecovery = 0;
                /// <summary> Возвращает или задаёт флаг нахождения героя. По умолчанию герой дома. <br/> true - герой дома, false - герой в походе или в подкрепе. </summary>
                public bool isHome = true;

                //поля не требующие сохранения в файл
                /// <summary> Хранит двумерный массив [100, 2]. <br/> Содержит 2 колонки: <br/> левая = уровень героя <br/> правая = сколько нужно убить юнитов героем чтобы повыситься до соответствующего уровня. </summary>
                /// <remarks> Поле не требует сохранения. </remarks>
                public int[,] Array_Hero_LVL_Kill = null;
                /// <summary> Хранит двумерный массив [151, 6] восстановления героя. <br/> Содиржит 6 колонок: <br/> [0] - уровень героя 0..150 <br/> [1][2][3][4] - расходы на восстановление: wood, clay, iron, crop <br/> [5] - время восстановления героя (double величина) </summary>
                ///<remarks> <inheritdoc cref="Array_Hero_LVL_Kill"/> </remarks>
                public double[,] Array_Hero_Recovery = null;

                /// <summary>
                ///     Метод сохраняет содержимое всех полей класса THero в файл: <b>Hero.DAT</b>, которые требуют сохранения. <br/> <br/>
                ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
                /// </summary>
                public void SaveHero(string path) {
                    using (FileStream fs = new FileStream($"{path}/Hero.DAT", FileMode.Create)) {
                        using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                            bw.Write(Registration_Village);    bw.Write(Name);            bw.Write(isCreated);
                            bw.Write(Level);                   bw.Write(Kills);           bw.Write(Points);
                            bw.Write(Points_per_lvl);          bw.Write(HP);              bw.Write(HP_Regeneration);
                            bw.Write(Power);                   bw.Write(Power_Add);       bw.Write(Bonus_Attack);
                            bw.Write(Bonus_Attack_Add);        bw.Write(Bonus_Defense);   bw.Write(Bonus_Defense_Add);
                            bw.Write(Bonus_Resourses.wood);    bw.Write(Bonus_Resourses.clay);
                            bw.Write(Bonus_Resourses.iron);    bw.Write(Bonus_Resourses.crop);
                            bw.Write(Bonus_Resourses.gold);
                            bw.Write(Bonus_Resourses_Add);     bw.Write(Bonus_Farm);      bw.Write(Bonus_Recovery_Time);
                            bw.Write(Bonus_Recovery_Time_Add); bw.Write(Consumption);     bw.Write(Speed);
                            bw.Write(Horse);                   bw.Write(Speed_Horse_Add); bw.Write(TimerOfRecovery);
                            bw.Write(isHome);
                    }}
                }

                /// <summary> 
                ///     Метод загружает в объект <b>Hero</b> данные из бинарного файла <b> Hero.DAT </b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
                ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
                /// </summary>
                public void LoadHero(string path) {
                    using (FileStream fs = new FileStream($"{path}/Hero.DAT", FileMode.Open)) {
                        using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                            Registration_Village = br.ReadString();    Name = br.ReadString();
                            isCreated = br.ReadBoolean();              Level = br.ReadInt32();
                            Kills = br.ReadInt32();                    Points = br.ReadInt32();
                            Points_per_lvl = br.ReadInt32();           HP = br.ReadInt32();    
                            HP_Regeneration = br.ReadInt32();          Power = br.ReadInt32(); 
                            Power_Add = br.ReadInt32();                Bonus_Attack = br.ReadDouble();
                            Bonus_Attack_Add = br.ReadDouble();        Bonus_Defense = br.ReadDouble();
                            Bonus_Defense_Add = br.ReadDouble();
                            Bonus_Resourses = new Res(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(),
                                                      br.ReadDouble(), br.ReadDouble());
                            Bonus_Resourses_Add = br.ReadInt32();      Bonus_Farm = br.ReadInt32();
                            Bonus_Recovery_Time = br.ReadDouble();     Bonus_Recovery_Time_Add = br.ReadDouble();
                            Consumption = br.ReadInt32();              Speed = br.ReadInt32();
                            Horse = br.ReadBoolean();                  Speed_Horse_Add = br.ReadInt32();
                            TimerOfRecovery = br.ReadInt32();          isHome = br.ReadBoolean();
                    }}
                }

                /// <summary> Метод проверяет: находится ли герой в войсках проверяемой деревни. </summary>
                /// <value> <b> <paramref name="ActiveVillageName"/>: </b> Название текущей активной деревни игрока. </value>
                /// <returns> Возвращает <b>true</b> если герой создан, дома, жив и находится в проверяемой деренве игрока <br/> и возвращает <b>false</b> если героя нет в войсках активной дерени. </returns>
                public bool InArmy(string ActiveVillageName) {
                    return (isCreated && isHome && HP > 0 && Registration_Village == ActiveVillageName);
                }
            }
            /// <summary> Поле класса TPlayer. <br/> Объект хранит все свойства героя аккаунта. <br/> Инициализируется в методе создания аккаунта: <b>TPlayer.CreateAccount(...);</b> </summary>
            public THero Hero = new THero();

            /// <summary> содержит одну загруженную в него деревню из файла со всеми её свойствами. </summary>
            /// <remarks>
            ///     Внимание! <br/>
            ///     После добавления нового поля в этом классе с номером строки для сохранения, сделать следующее: <br/>
            ///     - в методе <b>CreateVillage</b> в инициализации <b>Village</b> добавить созданное поле <br/>
            ///     - в папке <b>"DATA_BASE/json/"</b> в файлах <b>"Default_Village...json"</b> добавить строку с названием поля и значением в нужном месте <br/>
            ///     - в методе <b>LoadVillage</b> в инициализации <b>Village</b> добавить созданное поле <br/>
            ///     - в методе <b>SaveVillage()</b> добавить созданное поле <br/>
            /// </remarks>
            public partial class TVillage {
            //ПОЛЯ КЛАССА TVillage
            /// <summary> Возвращает или задаёт тип деревни. Значение default = 0. <br/> Это поле определяет какой грузить фон деревни: стандартный или натарский с чудом на вкладке "Деревня". </summary>
            /// <remarks> <inheritdoc cref = "TypeVillage"> </inheritdoc> </remarks>
            public TypeVillage Type_Village = TypeVillage.Other;

            /// <summary> Возвращает или задаёт название деревни аккаунта. </summary>
            public string Village_Name;
            /// <summary> Возвращает или задаёт тип ресурсных полей деревни (пример: "4-4-4-6" - 6ка или "1-1-1-15" - 15ка). </summary>
            public TypeCell Type_Resources;
            /// <summary> Возвращает или задаёт количество населения деревни. </summary>
            public int Population;
            /// <summary> Возвращает или задаёт одобрение деревни в процентах [0..100]. </summary>
            private int approval;
            /// <summary> 
            ///     <inheritdoc cref = "approval"> </inheritdoc> <br/>
            ///     Осуществляет проверку на корректность значения: <br/> если меньше нуля, то равно нулю, если больше 100, тогда равно 100.
            /// </summary>
            public int Approval {
                get { return approval; }
                set { if (value < 0) approval = 0; else if (value > 100) approval = 100; else approval = value;
            }}
            /// <summary> 
            ///     Возвращает или задаёт скорость восстановления одобрения до 100% за час в деревне. <br/>
            ///     Примечание: в папке IMG и в Help справке сайта есть формула восстановления одобрения. <br/>
            ///     Формула роста одобрения такова: в час прибавляется уровень резы/дворца.
            /// </summary>
            public uint Add_Approval;
            /// <summary>
            ///     Возвращает или задаёт накопленные единицы культуры деревней. <br/>
            ///     Поле Culture_EK суммируется "+=" с полем add_Culture_EK либо раз в сутки, либо раз в сек. порциями. <br/>
            ///     Чтобы узнать сумму накопленных ед. культуры аккаунта, надо или сложить все поля <b> Culture_EK </b> каждой деревни, или посмотреть поле <b> Player.Culture_EK </b> <br/><br/>
            /// </summary>
            private uint ek = 0;
            /// <summary> <inheritdoc cref = "ek"> </inheritdoc> </summary>
            public uint Culture_EK { get { return ek; } set { ek = value; } }
            /// <summary> 
            ///     Возвращает или задаёт скорость прироста культуры в деревне. <br/>
            ///     На эту величину культура увеличивается раз в сутки (86400 сек). <br/>
            ///     Поле суммируется "+=" с величиной культуры только что завершённой пострйки (из файла). <br/>
            ///     Чтобы узнать сумму прироста культуры аккаунта, надо или сложить все поля <b> add_Culture_EK </b> каждой деревни, или посмотреть поле <b> Player.add_Culture_EK </b> <br/><br/>
            /// </summary>
            public uint Add_Culture_EK = 0;
            /// <summary> Возвращает или задаёт координаты деревни в системе координат мира Travian: +/- с центром (0, 0) на карте. </summary>
            private Point коры;
            /// <summary> <inheritdoc cref = "коры"> </inheritdoc> </summary>
            public Point Coordinates_World_Travian { get { return коры; } set { коры = value; } }
            /// <summary> Возвращает или задаёт координаты деревни в системе координат массива: Map.Cell[x][y]. </summary>
            public Point Coordinates_Cell;
            /// <summary> 
            ///     Возвращает или задаёт выработку ресурсов в час в деревне. <br/>
            ///     Это чистая выработка без учёта потребления зерна деревней и бонусами % производства. <br/>
            ///     Итоговая выработка с учётом потребления и бонусами % учитывается в EventTimers и в отрисовке панели добыичи ресурсов.
            /// </summary>
            public Res HourlyProductionResources;
            /// <summary>
            ///     Возвращает или задаёт проценты к выработке ресурсов в час. <br/>
            ///     Пример: <br/> HourlyProductionResources.wood = 100; <br/> HourlyProductionResources_PercentOfIncrease.wood = 25% <br/>
            ///     HourlyProductionResources.wood = 125; //на панели добычи и при рисчётах. <br/>
            ///     HourlyProductionResources.wood = 100; //по скольку в этом поле хранится чистая выработка, то хранится 100.
            /// </summary>
            public Res HourlyProductionResources_PercentOfIncrease;
            /// <summary> Возвращает или задаёт количество ресурсов в хранилищах деревни. </summary>
            private Res resources_in_storages;
            /// <summary>  <inheritdoc cref = "resources_in_storages"> </inheritdoc> <br/> Осуществляет проверку переполнения хранилищ. </summary>
            /// <remarks> 
            ///     <inheritdoc cref = "Index"> </inheritdoc>
            ///     <code>
            ///         АХТУНГ! Если присваивать ресурсы так:<br/> <b>ResourcesInStorages.wood = 100;</b> <br/>
            ///         То не будет проверки на переполнение хранилищ и отрицательные значения. <br/>
            ///         Следует присваивать ресурсы так: <br/>
            ///         (ResourcesInStorages = RES - для сокращения записи!) <br/>
            ///         <b>1) RES.crop += 525; RES.gold += 20;
            ///         RES = new Res(RES.wood, RES.clay, RES.iron, RES.crop, RES.gold);
            ///         2) RES = new Res(RES.wood -= 160, RES.clay - 85, RES.iron -= 120, RES.crop - 25, RES.gold);</b>
            ///     </code>
            /// </remarks>
            public Res ResourcesInStorages { 
                get { return resources_in_storages; } 
                set { resources_in_storages = CheckOverflowAll(value); }
            }
            /// <summary> Метод обрабатывает переполнение Склада, Амбара и Казны. </summary>
            /// <value> <b> res: </b> структура хранящая количество ресурсов. </value>
            /// <returns> Возвращает структуру с откорректированным количеством ресурсов в хранилищах. </returns>
            private Res CheckOverflowAll(Res res) {
                res.wood = CheckOverflow_Warehouse(res.wood); res.clay = CheckOverflow_Warehouse(res.clay);
                res.iron = CheckOverflow_Warehouse(res.iron); res.crop = CheckOverflow_Barn(res.crop);
                res.gold = CheckOverflow_Treasury(res.gold); return res;
            }
            /// <summary> Метод обрабатывает переполнение склада. </summary>
            /// <value> <b> res: </b> количество ресурса. </value>
            /// <returns> Откорректированное количество ресурса в хранилище. </returns>
            private double CheckOverflow_Warehouse(double res) { return Capacity.Warehouse < res ? Capacity.Warehouse : res < 0 ? 0 : res; }
            /// <summary> Метод обрабатывает переполнение амбара. </summary>
            /// <value> <inheritdoc cref="CheckOverflow_Warehouse"> </inheritdoc> </value>
            /// <returns> <inheritdoc cref="CheckOverflow_Warehouse"> </inheritdoc> </returns>
            private double CheckOverflow_Barn(double res) { return Capacity.Barn < res ? Capacity.Barn : res < 0 ? 0 : res; }
            /// <summary> Метод обрабатывает переполнение казны. </summary>
            /// <value> <inheritdoc cref="CheckOverflow_Warehouse"> </inheritdoc> </value>
            /// <returns> <inheritdoc cref="CheckOverflow_Warehouse"> </inheritdoc> </returns>
            private double CheckOverflow_Treasury(double res) { return Capacity.Treasury < res ? Capacity.Treasury : res < 0 ? 0 : res; }

            /// <summary> 
            ///     Возвращает или задаёт текущую вместимость всех хранилищ деревни (склад, амбар, казна). <br/>
            ///     А так же хранит текущую вместимость для остальных построек: (посольство, тайник)
            /// </summary>
            public sCapacity Capacity;
            /// <summary> Возвращает или задаёт общее потребление зерна в час в деревне. </summary>
            private int crop_consumption;
            /// <summary> <inheritdoc cref = "crop_consumption"> </inheritdoc> </summary>
            public int Crop_Consumption { get { return crop_consumption; } set { crop_consumption = value; } }
            /// <summary> Возвращает или задаёт общее количество ловушек у Галлов в деревне. </summary>
            private int quantity_of_traps_total;
            /// <summary> <inheritdoc cref = "quantity_of_traps_total"> </inheritdoc> </summary>
            public int QuantityOfTraps_Total { get { return quantity_of_traps_total; } set { quantity_of_traps_total = value; } }
            /// <summary> Возвращает или задаёт количество свободных для постройки ловушек у Галлов в деревне. </summary>
            private int quantity_of_traps_available;
            /// <summary> <inheritdoc cref = "quantity_of_traps_available"> </inheritdoc> </summary>
            public int QuantityOfTraps_Available { get { return quantity_of_traps_available; } set { quantity_of_traps_available = value; } }
            /// <summary> Возвращает или задаёт количество построенных ловушек у Галлов в деревне. </summary>
            private int quantity_of_traps_built;
            /// <summary> <inheritdoc cref = "quantity_of_traps_built"> </inheritdoc> </summary>
            public int QuantityOfTraps_Built { get { return quantity_of_traps_built; } set { quantity_of_traps_built = value; } }
            /// <summary> Возвращает или задаёт количество свободных торговцев в деревне. </summary>
            private int quantity_merchants_available;
            /// <summary> <inheritdoc cref = "quantity_merchants_available"> </inheritdoc> </summary>
            public int QuantityMerchants_Available { get { return quantity_merchants_available; } set { quantity_merchants_available = value; } }
            /// <summary> Возвращает или задаёт общее количество торговцев в деревне. </summary>
            private int quantity_merchants_total;
            /// <summary> <inheritdoc cref = "quantity_merchants"> </inheritdoc> </summary>
            public int QuantityMerchants_Total { get { return quantity_merchants_total; } set { quantity_merchants_total = value; } }
            /// <summary> 
            ///     Возвращает или задаёт количество раненых юнитов в деревне которых можно вылечить в госпитале и вернуть в строй. <br/>
            ///     Хранит массив на 10 элементов по количеству юнитов нации: Wounded_Units[0..9]; <br/>
            ///     Пример использования: <br/>
            ///     <b> Wounded_Units[0] = 100; </b> <br/>
            ///     //У нации Римляне: 100 легионеров ранено. <br/> //У нации Германцы: 100 дубинщиков ранено. <br/> //У нации Галлы: 100 фаланг ранено. <br/> 
            /// </summary>
            public int[] Wounded_Units = new int[10];
            /// <summary>
            ///     Возвращает или задаёт множитель цены за лечение одного раненого воина в золоте в деревне. Множитель умножается на потребление зерна воином. <br/>
            ///     Пример: <br/> <b> Treatment_Cost_Multiplier = 33; <br/> Wounded_Units[N] = 100; <br/> Юнит Конница Цезаря, потребление 4 зерна. <br/> Золотых будет потрачено: 33 * 100 * 4 = 13'200 gold. </b>
            /// </summary>
            public double Wounded_TreatmentCostMultiplier;
            /// <summary>
            ///     Возвращает или задаёт процент умирающих юнитов в сутки в деревне <b> без округления. </b> <br/> Если Госпиталь отсутствует, умирать будут все воины мгновенно. <br/>
            ///     За сколько дней умрут все N юнитов на максимуме и минимуме процентов? Пример: <br/><br/>
            ///     <b>MAX</b> <br/>
            ///     Wounded_DeathOfUnitsPerDay_Percent = 50% <br/> Wounded_Units[N] = 100 (паладинов); <br/>
            ///     [1] Сутки. Wounded_Units[N] = 100 - 50% = 50 (паладинов) <br/>
            ///     Далее: [2]=25, [3]=12, [4]=6, [5]=3, [6]=1, [7]=0 <br/><br/>
            ///     <b>MIN</b> <br/>
            ///     Wounded_DeathOfUnitsPerDay_Percent = 10% <br/> Wounded_Units[N] = 100 (паладинов); <br/>
            ///     [1] Сутки. Wounded_Units[N] = 100 - 10% = 90 (паладинов) <br/>
            ///     Далее: [2]=81, [3]=72, [4]=64, [5]=57, [6]=51, [7]=45, [8]=40, [9]=36, [10]=32, [11]=28, [12]=25, [13]=22, [14]=19, <br/>
            ///     [15]=17, [16]=15, [17]=13, [18]=11, [19]=9, [20]=8, [21]=7, [22]=6, [23]=5, [24]=4, [25]=3, [26]=2, [27]=1, [28]=0 <br/><br/>
            ///     Из-за отсутствия округления при вычите % из 1 юнита, результат будет 0 раненых юнитов. <br/> Чем больше раненых юнитов, тем дольше они будут умирать.
            /// </summary>
            public double Wounded_DeathOfUnitsPerDay_Percent;
            /// <summary> Возвращает или задаёт бонус Арены. + % к скорости передвижения юнитов из деревни с ареной по карте. <br/> Подробнее в файле g14-ltr.rtf </summary>
            public int BonusOfSpeed_Arena;
            /// <summary>
            ///     Возвращает или задаёт бонус Главного Здания. -% к времени строительства. Он учитывается при запуске постройки. <br/> 
            ///     Главное здание 1 уровня снести нельзя, но можно уничтожить катапультами. <br/> В этом случае в симуляции битвы пишем Бонус_Времени_строительства = 0. <br/>
            /// </summary>
            /// <remarks>
            ///     Пример использования бонуса: <br/>
            ///     Постройка с бонусом 10 lvl = бонус 72% времени от номинала. <br/> Время строительства/обучения 02:16:30 ч. = 8190 сек. <br/>
            ///     новое время = 8190 / 100 * 72 = 5896,8 сек = 01:38:16 ч. <br/> округлять double или нет по барабану. <br/>
            /// </remarks>
            public int BonusOfTime_Construction;
            /// <summary> Возвращает или задаёт максимальное текущее количество отображаемых передвижений (слотов) в пункте сбора. </summary>
            public int QuantityOfVisibleMovements_CollectionPoint;
            /// <summary> Возвращает или задаёт бонус казармы. -% к времени обучения. Он учитывается при добавлении войск на обучение. </summary>
            /// <remarks> <inheritdoc cref = "BonusOfTime_Construction"> </inheritdoc> </remarks>
            public int BonusOfTimeTraining_Barrack;
            /// <summary> Возвращает или задаёт бонус конюшни. -% к времени обучения. Он учитывается при добавлении войск на обучение. </summary>
            /// <remarks> <inheritdoc cref = "BonusOfTime_Construction"> </inheritdoc> </remarks>
            public int BonusOfTimeTraining_Stable;
            /// <summary> Возвращает или задаёт бонус мастерской. -% к времени обучения. Он учитывается при добавлении войск на обучение. </summary>
            /// <remarks> <inheritdoc cref = "BonusOfTime_Construction"> </inheritdoc> </remarks>
            public int BonusOfTime_TrainingInWorkshop;
            /// <summary>
            ///     Возвращает или задаёт текущее количество СВОБОДНЫХ слотов экспансии в деревне. <br/> 
            ///     Новая деревня захватывается/основывается так: <br/>
            ///     ЕСЛИ <b> ExpansionSlots_Free </b> больше чем <b> ExpansionSlots_Occupied </b> ТОГДА основываем/захватываем деревню и инкрементируем поле <b> ExpansionSlots_Occupied++; </b> <br/>
            ///     Если <b> ExpansionSlots_Free </b> меньше или равно <b> ExpansionSlots_Occupied </b> ТОГДА достигнут текущий лимит новых деревень
            /// </summary>
            public int ExpansionSlots_Free;
            /// <summary> Возвращает или задаёт текущее количество ЗАНЯТЫХ слотов экспансии в деревне. </summary>
            public int ExpansionSlots_Occupied;
            /// <summary> Возвращает или задаёт бонус торговой палаты. +% к грузоподъёмности торговцев. Он учитывается при отправе торговцев в другие деревни и торговле. </summary>
            public int BonusOfCapacity_Merchants;
            /// <summary> Возвращает или задаёт бонус большой казармы. -% к времени обучения. Он учитывается при добавлении войск на обучение. </summary>
            /// <remarks> <inheritdoc cref = "BonusOfTime_Construction"> </inheritdoc> </remarks>
            public int BonusOfTimeTraining_BigBarrack;
            /// <summary> Возвращает или задаёт бонус большой конюшни. -% к времени обучения. Он учитывается при добавлении войск на обучение. </summary>
            /// <remarks> <inheritdoc cref = "BonusOfTime_Construction"> </inheritdoc> </remarks>
            public int BonusOfTimeTraining_BigStable;
            /// <summary> Возвращает или задаёт бонус защиты стены. +% к общему кол-ву очков защиты воинов защищающих деревню. Он учитывается при расчёте битвы. </summary>
            public int BonusOfProtection_Wall;
            /// <summary> Возвращает или задаёт кол-во таранов которые снесут стену полностью с первого раза при условии если все тараны выжили. </summary>
            public int CompleteDestruction_Wall_CountOfRams;
            /// <summary> Возвращает или задаёт бонус дома каменотёса. +% к устойчивости зданий перед катапультами. Он учитывается при расчёте битвы в которой участвуют катапульты. </summary>
            public int BonusOfStability_Buildings;
            /// <summary>
            ///     Возвращает или задаёт текущее количество СВОБОДНЫХ слотов таверны в деревне. <br/>
            ///     Новый оазис захватывается по таким же условиям как и в поле <b> FreeExpansionSlots. </b>
            /// </summary>
            public int OasisSlots_Free;
            /// <summary> Возвращает или задаёт текущее количество ЗАНЯТЫХ слотов таверны в деревне. </summary>
            public int OasisSlots_Occupied;
            /// <summary> Возвращает или задаёт текущий бонус водопоя (Римляне). -% к скорости обучения конных войск в конюшне. Он учитывается при добавлении войск на обучение. </summary>
            public int BonusOfTimeTraining_WateringHole;
            /// <summary> Возвращает или задаёт текущий бонус потребления зерна в час римским <b> конным войскам. </b> У всех наций в этом поле <b> ZERO </b> кроме Римлян. </summary>
            /// <remarks> При <b> водопое 10 lvl </b> это поле получает значение <b> -1; </b> что уменьшает потребление зерна в час на эту величину <b> конным разведчикам </b>. </remarks>
            public int Bonus_OfCropConsumption_ByMountedScouts_ROMANS;
            /// <summary> <inheritdoc cref = "BonusOfTimeTraining_WateringHole"> </inheritdoc> </summary>
            /// <remarks> При <b> водопое 15 lvl </b> это поле получает значение <b> -1; </b> что уменьшает потребление зерна в час на эту величину <b> коннице императора </b>. </remarks>
            public int Bonus_OfCropConsumption_ByEmperorCavalry_ROMANS;
            /// <summary> <inheritdoc cref = "BonusOfTimeTraining_WateringHole"> </inheritdoc> </summary>
            /// <remarks> При <b> водопое 20 lvl </b> это поле получает значение <b> -1; </b> что уменьшает потребление зерна в час на эту величину <b> коннице Цезаря </b>. </remarks>
            public int Bonus_OfCropConsumption_By_CaesarCavalry_ROMANS;
            /// <summary> Возвращает или задаёт время продолжительности <b> маленького праздника </b> в ратуше на данном уровне строения. </summary>
            public int DurationOfHoliday_TownHall;
            /// <summary> Возвращает или задаёт время продолжительности <b> торжества </b> в ратуше на данном уровне строения. </summary>
            public int DurationOfCelebration_TownHall;
            /// <summary>
            ///     Возвращает или задаёт таймер <b> маленького праздника </b> отсчитывающий обратное время. Если таймер больше 0, значит праздник запущен. <br/>
            ///     После истечения времени в деревню (и аккаунт) добавляются очки культуры <b> ЕК (500 ед.) </b> сразу.
            /// </summary>
            public int Timer_Holiday_TownHalls;
            /// <summary>
            ///     Возвращает или задаёт таймер <b> торжества </b> отсчитывающий обратное время. Если таймер больше 0, значит праздник запущен. <br/>
            ///     После истечения времени в деревню (и аккаунт) добавляются очки культуры <b> ЕК (2000 ед.) </b> сразу.
            /// </summary>
            public int Timer_Celebration_TownHalls;
            /// <summary> Хранит список координат оазисов которые принадлежат данной деревне. Координаты хранятся в системе координат <b>Map.Cell[x][y];</b> </summary>
            public List<Point> OasisList = new List<Point>();

            /// <summary>
            ///     Метод сохраняет поля класса <b>TVillage</b> в файлы: <b>Village[N].DAT</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
            /// </summary>
            /// <remarks> <b>OasisList</b> сохраняется в этом методе т.к. он принадлежит деревне. </remarks>
            public void SaveVillage(string path) {
                using (FileStream fs = new FileStream($"{path}/Village [{Village_Name}].DAT", FileMode.CreateNew)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(Village_Name); /*1*/
                        bw.Write((int)Type_Resources);/*2*/bw.Write(Population);/*3*/
                        bw.Write(Approval);     /*4*/     bw.Write(Add_Approval);/*5*/
                        bw.Write(Culture_EK);   /*6*/     bw.Write(Add_Culture_EK);/*7*/
                        bw.Write(Coordinates_World_Travian.X);/*8*/     bw.Write(Coordinates_World_Travian.Y);/*9*/
                        bw.Write(Coordinates_Cell.X);/*8*/              bw.Write(Coordinates_Cell.Y);/*9*/
                        bw.Write(HourlyProductionResources.wood);/*10*/ bw.Write(HourlyProductionResources.clay);/*11*/
                        bw.Write(HourlyProductionResources.iron);/*12*/ bw.Write(HourlyProductionResources.crop);/*13*/
                        bw.Write(HourlyProductionResources.gold);/*14*/
                        bw.Write(Capacity.Warehouse);/*15*/          bw.Write(Capacity.Barn);/*16*/
                        bw.Write(Capacity.Treasury);/*17*/           bw.Write(Capacity.Embassy);/*18*/
                        bw.Write(Capacity.Stash);/*19*/
                        bw.Write(ResourcesInStorages.wood);/*20*/    bw.Write(ResourcesInStorages.clay);/*21*/
                        bw.Write(ResourcesInStorages.iron);/*22*/    bw.Write(ResourcesInStorages.crop);/*23*/
                        bw.Write(ResourcesInStorages.gold);/*24*/
                        bw.Write(Crop_Consumption);/*25*/ bw.Write(QuantityOfTraps_Total);/*26*/
                        bw.Write(QuantityOfTraps_Available);/*27*/   bw.Write(QuantityOfTraps_Built);/*28*/
                        bw.Write(QuantityMerchants_Available);/*29*/ bw.Write(QuantityMerchants_Total);/*30*/
                        bw.Write(HourlyProductionResources_PercentOfIncrease.wood);/*31*/
                        bw.Write(HourlyProductionResources_PercentOfIncrease.clay);/*32*/
                        bw.Write(HourlyProductionResources_PercentOfIncrease.iron);/*33*/
                        bw.Write(HourlyProductionResources_PercentOfIncrease.crop);/*34*/
                        bw.Write(HourlyProductionResources_PercentOfIncrease.gold);/*35*/
                        bw.Write(Wounded_Units[0]);/*36*/  bw.Write(Wounded_Units[1]);/*37*/  bw.Write(Wounded_Units[2]);/*38*/
                        bw.Write(Wounded_Units[3]);/*39*/  bw.Write(Wounded_Units[4]);/*40*/  bw.Write(Wounded_Units[5]);/*41*/
                        bw.Write(Wounded_Units[6]);/*42*/  bw.Write(Wounded_Units[7]);/*43*/  bw.Write(Wounded_Units[8]);/*44*/
                        bw.Write(Wounded_Units[9]);/*45*/
                        bw.Write(Wounded_TreatmentCostMultiplier);/*46*/ bw.Write(Wounded_DeathOfUnitsPerDay_Percent);/*47*/
                        bw.Write(BonusOfSpeed_Arena);/*48*/              bw.Write(BonusOfTime_Construction);/*49*/
                        bw.Write(QuantityOfVisibleMovements_CollectionPoint);/*50*/ bw.Write(BonusOfTimeTraining_Barrack);/*51*/
                        bw.Write(BonusOfTimeTraining_Stable);/*52*/      bw.Write(BonusOfTime_TrainingInWorkshop);/*53*/
                        bw.Write(ExpansionSlots_Free);/*54*/             bw.Write(ExpansionSlots_Occupied);/*55*/
                        bw.Write(BonusOfCapacity_Merchants);/*56*/       bw.Write(BonusOfTimeTraining_BigBarrack);/*57*/ 
                        bw.Write(BonusOfTimeTraining_BigStable);/*58*/   bw.Write(BonusOfProtection_Wall);/*59*/ 
                        bw.Write(CompleteDestruction_Wall_CountOfRams);/*60*/ bw.Write(BonusOfStability_Buildings);/*61*/
                        bw.Write(OasisSlots_Free);/*62*/ bw.Write(OasisSlots_Occupied);/*63*/
                        bw.Write(BonusOfTimeTraining_WateringHole);/*64*/
                        bw.Write(Bonus_OfCropConsumption_ByMountedScouts_ROMANS);/*65*/
                        bw.Write(Bonus_OfCropConsumption_ByEmperorCavalry_ROMANS);/*66*/
                        bw.Write(Bonus_OfCropConsumption_By_CaesarCavalry_ROMANS);/*67*/
                        bw.Write(DurationOfHoliday_TownHall);/*68*/ bw.Write(DurationOfCelebration_TownHall);/*69*/
                        bw.Write(Timer_Holiday_TownHalls);/*70*/    bw.Write(Timer_Celebration_TownHalls);/*71*/
                        bw.Write((int)Type_Village);/*74*/
                        //лист координат оазисов
                        if (OasisList.Count <= 0 ) bw.Write(-1);//-1 = отсутствие списка, >=1 = Count
                        else { bw.Write(OasisList.Count);
                            for (int i = 0; i < OasisList.Count; i++ ) {
                                bw.Write(OasisList[i].X); bw.Write(OasisList[i].Y);
                            }
                        } 
                    }
                }
            }

            /// <summary> хранит базовую информацию по постройкам и ресурсным полям в деревне в каждом слоте. </summary>
                public class TSlot {
                //ПОЛЯ КЛАССА Info
                //поля для внутреннего пользования. не знаю как скрыть эти методы для экземпляра класса.
                /// <summary> Возвращает или задаёт текущий уровень ID в конкретном слоте. (<b> уровень </b> = [0..20]). </summary>
                private int level = 0;
                /// <summary> <inheritdoc cref = "level"> </inheritdoc> </summary>
                public int Level { get { return level; } set { level = value; } }
                /// <summary>
                /// <b> ID: </b> номер построенной постройки в слоте. Диапазон = [0..40]. Всего 41 шт. <br/>
                ///     [0] - Чудо, [1..4] - ресурсные постройки, [5..40] - деревенские. <br/>
                ///     Номер ID соответствует номеру файлов в папке "DATA_BASE\IMG\building". <br/>
                ///     Если ID = -1, значит слот пуст (актуально только для деревенских слотов) <br/>
                ///     <b> Пример: </b> <br/>
                ///         При ID = 4, загрузятся все файлы ресурсов с названием "g4-ltr." + png/gif/rtf/txt. (Ферма) <br/>
                ///         При ID = 25, загрузятся все файлы ресурсов с названием "g25-ltr." + png/gif/rtf/txt. (Резиденция) <br/> и т.д. <br/>
                ///     Ещё по номеру <b>ID</b> можно получить имя постройки из файла LANGUAGES/[ЯЗЫК]/buildings.txt: <br/>
                ///     <b> ID </b> = 18 ("Посольство"), <b> ID </b> = 25 ("Резиденция"), <b> ID </b> = 4 ("Ферма") и т.д. <br/>
                /// </summary>
                private Buildings id;
                /// <summary> <inheritdoc cref = "id"> </inheritdoc> </summary>
                public Buildings ID { get { return id; } set { id = value; } }
                /// <summary> 
                ///     Хранит информацию о том: идёт ли в данном слоте строительство.
                ///     <b>true = </b> стройка идёт, <b>false = </b> строительство завершено или слот пустой. <br/>
                ///     Это нужно для понимания того, какую грузить картинку для здания в слоте: <b>.gif</b> или <b>.png</b> <br/> 
                ///     Актуально только для слотов деревни. Для ресурсных слотов [0..17] нет картинок "в процессе строительства" !!! <br/>
                ///     <b>ПОЛЕ ДЛЯ РЕСУРСНЫХ СЛОТОВ И СТЕН НЕ ИСПОЛЬЗУЕТСЯ И ВСЕГДЯ = FALSE; У ЧУДА НЕСКОЛЬКО ПРОМЕЖУТОЧНЫХ ВАРИАНТОВ.</b> <br/><br/>
                ///     Пример использования: <br/>
                ///     <code><b>
                ///         if (VillageList[0].Slot[20].ProcessOfConstruction) грузим ".gif"; //в деревне с индексом 0, в слоте 20 идёт строительство. <br/>
                ///         else if (VillageList[0].Slot[20].ID == -1) грузим картинку <b>null;</b> //слот пустой. <br/>
                ///         else грузим ".png"; //в деревне с индексом 0, в слоте 20 есть постройка и она в данный момент не строится. <br/>
                ///         ID = 25; //в данном случае грузим резиденцию [25] <br/> 
                ///         Control.BackGroundImage = Image.FromFile("DATA_BASE\IMG\building\g" + ID + "-ltr." + ...); //в зависимости от условий грузим либо <b>.gif</b>, либо <b>.png</b>, либо картинку <b>null</b>
                ///     </b></code>
                ///     Какую картинку грузить, зависит от поля <b>ID</b>. В нём хранится номер загружаемой картинки.
                /// </summary>
                private bool process_of_construction = false;
                /// <summary> <inheritdoc cref = "process_of_construction"> </inheritdoc> </summary>
                public bool ProcessOfConstruction { get { return process_of_construction; } set { process_of_construction = value; } }
                /// <summary> Хранит номер пострйоки в слоте в процессе строительства. Отвечает на вопрос: "Что сейчас строится?" </summary>
                public Buildings ID_ProcessOfConstruction;

                /// <summary>
                ///     Сохранение полей класса <b>TInfo</b> в файлы: <b>Village[N].DAT</b> <br/> <br/>
                ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
                ///     <b> <paramref name="Village_Name"/>: </b> название деревни. <br/>
                /// </summary>
                public void SaveSlot(string path, string Village_Name) {
                    using (FileStream fs = new FileStream($"{path}/Village [{Village_Name}].DAT", FileMode.Append)) {
                        using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                            //каждый слот сохраняет эти поля в конец файла Village.DAT друг за другом
                            bw.Write(Level);                    bw.Write((int)ID);
                            bw.Write(ProcessOfConstruction);    bw.Write((int)ID_ProcessOfConstruction);
                    }}
                }
            }
            /// <summary>
            ///     <b> Slot: </b> место для ресурсных и деревенских построек. <br/>
            ///     Всего 40 слотов: 18 ресурсных + 22 здания. [0..17] - ресурсные постройки, [18..39] - деревенские постройки. <br/>
            ///     Зарезервированные номера слотов в деревне: [18] - стена [19] = пункт сбора [27] - главное здание.
            /// </summary>
            /// <remarks>
            ///     Пример записи кода: <br/>
            ///     <code>
            ///         Player.VillageList[0].Slot[10].Level = 5;
            ///         Player.VillageList[0].Slot[10].ID = <b>2;</b> (Глиняный карьер) <br/>
            ///         Player.VillageList[0].Slot[30].ProcessOfConstruction = false;
            ///         Результат: в деревне 0, в слоте 10 глиняный карьер 5 уровня, в слоте нет строительства. (загрузятся ресурсы из "DATA_BASE\IMG\building\g<b>2</b>-ltr.", -"png/gif/rtf/txt").
            ///         
            ///         Player.VillageList[0].Slot[30].Level = 8;
            ///         Player.VillageList[0].Slot[30].ID = <b>23;</b> (Тайник) <br/>
            ///         Player.VillageList[0].Slot[30].ProcessOfConstruction = true;
            ///         Результат: в деревне 0, в слоте 30 тайник 8 уровня в процессе строительства до 9 уровня. (загрузятся ресурсы из "DATA_BASE\IMG\building\g<b>23</b>-ltr.", -"png/gif/rtf/txt").
            ///     </code>
            /// </remarks>
            public TSlot[] Slot = new TSlot[18 + 22];
        }
        /// <summary> 
        ///     Поле класса TPlayer. <br/> Содержит список всех деревень данного аккаунта. <br/>
        ///     Каждая строка <b> VillageList </b> является объектом класса <b> TVillage </b> <br/>
        ///     который <inheritdoc cref="TVillage"> </inheritdoc>
        /// </summary>
        public List<TVillage> VillageList = new List<TVillage>();//настройки списка всех деревень данного аккаунта



        //ПОЛЯ КЛАССА Player
        /// <summary> Возвращает или задаёт название папки с именем аккаунта в которой хранятся все файлы аккаунта. </summary>
        public string FolderAccount = "";

        //поля загружаются из файла account.DAT
        /// <summary> Возвращает или задаёт игровой ник аккаунта. </summary>
        public string Nick_Name = "";
        /// <summary> Возвращает или задаёт ранг аккаунта по суммарному населению для статистики. </summary>
        private int rank = 0;
        /// <summary> <inheritdoc cref = "rank"> </inheritdoc> </summary>
        public int Rank {
            get { return rank; }
            set { //позже здесь будет написан код вычисляющий ранг аккаунта.
                  //в цикле будет проверка по общему населению (поле folk_population)
                rank = value;
        }}
        /// <summary> Возвращает или задаёт название нации/цивилизации/народа/этноса этого аккаунта. </summary>
        private Folk folk_name = Folk.NULL;
        /// <summary> <inheritdoc cref = "folk_name"> </inheritdoc> </summary>
        public Folk Folk_Name { get { return folk_name; } set { folk_name = value; } }
        /// <summary> Возвращает текущее максимальное количество деревень которое может иметь аккаунт. </summary>
        private uint limit_village = 0;
        /// <summary> <inheritdoc cref = "limit_village"> </inheritdoc> </summary>
        /// <value> 
        ///     Вычисляется автоматически и доступно только для чтения. 
        ///     В свойстве <b> set </b> поля <b> Culture_EK </b> срабатывает вызов метода <b> Check_Limit_Village(); </b> <br/>
        ///     который вычисляет новое значение <b> limit_village </b> всякий раз когда измененяются единицы культуры аккаунта.
        /// </value>
        public uint Limit_Village { get { return limit_village; } private set { limit_village = value; } }
        /// <summary> 
        ///     Хранит активную (выбранную) деревню аккаунта. Величина <b> ActiveIndex </b> соответствует номеру файла Village<b>N</b>.DAT, <br/>
        ///     где <b>N</b> - это порядковый номер файлов <b>Village</b> с цифрой на конце (файлы нумеруются с нуля). <br/>
        ///     Величина <b>ActiveIndex</b> не совпадает с величиной списка деревень <b>ListBox1.SelectIndex</b> на экране! <br/>
        ///     Значение <b>ActiveIndex</b> вычисляется при выборе деревни в <b>ListBox1</b> в окне главной формы.
        /// </summary>
        private int activeindex = 0;
        /// <summary> 
        ///     <inheritdoc cref = "activeindex"> </inheritdoc> <br/>
        ///     Осуществляет проверку на корректность значения: если меньше нуля, то равно нулю. <br/><br/>
        ///     Модификатор доступа к свойству <b>set</b> приватен из-за более сложной проверки, <br/>
        ///     требующей дополнительного параметра получаемого из вне. Свойство <b>set</b> его получить неспособно. <br/><br/>
        ///     Чтобы изменить значение этого поля, используйте <b>public</b> методы: <br/>
        ///     <b>ActiveIndex_set_Village;</b> и <b>ActiveIndex_set_коры;</b>
        /// </summary>
        /// <remarks> <inheritdoc cref = "Nick_Name"> </inheritdoc> </remarks>
        public int ActiveIndex {
            get { return activeindex; }
            private set { if (value >= VillageList.Count || value < 0) { activeindex = 0; } else activeindex = value;
        }}
        /// <summary> 
        ///     метод принимает на вход строку из <b>ListBox1</b> с названием деревни, <br/>
        ///     ищет порядковый номер деревни в списке деревень <b>VillageList[i]</b> с таким же именем <br/>
        ///     и присваивает полученное значение <b>[i]</b> в цикле полю <b>ActiveIndex</b>. <br/>
        ///     Если строка <b>VillageName</b> пуста или равна <b>null</b>, то поле <b>ActiveIndex</b> = 0.
        /// </summary>
        public void ActiveIndex_set_Village(string VillageName) {
            if (VillageName == "" || VillageName == null) { ActiveIndex = 0; return; }
            for (int i = 0; i < VillageList.Count; i++)
                if (VillageName == VillageList[i].Village_Name) { ActiveIndex = i; break; }
        }
        /// <summary> 
        ///     метод принимает на вход строку из <b>ListBox2</b> с координатами деревни, <br/>
        ///     ищет порядковый номер деревни в списке деревень <b>VillageList[i]</b> с такими же координатами <br/>
        ///     и присваивает полученное значение <b>[i]</b> в цикле полю <b>ActiveIndex</b>. <br/>
        ///     Если строка <b>VillageName</b> пуста или равна <b>null</b>, то поле <b>ActiveIndex</b> = 0.
        /// </summary>
        public void ActiveIndex_set_Коры(string VillageКоры) {
            if (VillageКоры == "" || VillageКоры == null) { ActiveIndex = 0; return; }
            for (int i = 0; i < VillageList.Count; i++)
                if (VillageКоры == "(" + VillageList[i].Coordinates_World_Travian.X.ToString() + "|"
                                       + VillageList[i].Coordinates_World_Travian.Y.ToString() + ")")
                    { ActiveIndex = i; break; }
        }
        /// <summary> Хранит суммарное население всех деревень аккаунта. </summary>
        private int total_population = 2;
        /// <summary> <inheritdoc cref = "total_population"> </inheritdoc> </summary>
        public int Total_Population { get { return total_population; } set { total_population = value; } }
        /// <summary> Хранит номер столицы в листе деревень аккаунта. По умолчанию столицей является первая деревня в списке. </summary>
        private uint number_of_the_capital = 0;
        /// <summary> <inheritdoc cref = "number_of_the_capital"> </inheritdoc> </summary>
        public uint NumberOfCapital { get { return number_of_the_capital; } set { number_of_the_capital = value; } }
        /// <summary> 
        ///     Хранит общую сумму накопленных единиц культуры во всех деревнях аккаунта. <br/>
        ///     Поле Culture_EK суммируется "+=" с полем add_Culture_EK либо раз в сутки, либо раз в сек. порциями.
        /// </summary>
        private uint ek = 0;
        /// <summary> 
        ///     <inheritdoc cref = "ek"> </inheritdoc> <br/> С изменением поля <b>Culture_EK</b>, вычисляется новое значение поля <b>Limit_Village</b>. <br/>
        ///     <b>Limit_Village:</b> <inheritdoc cref = "Limit_Village"/>
        /// </summary>
        public uint EK { get { return ek; } set { ek = value; Check_Limit_Village(); } }
        /// <summary> 
        ///     Хранит скорость прироста культуры во всех деревнях аккаунта. <br/>
        ///     На эту величину культура увеличивается раз в сутки (86400 сек). <br/>
        ///     Поле является суммой "+=" всех полей <b> add_Culture_EK </b> каждой деревни. <br/>
        /// </summary>
        private uint add_ek = 2;
        /// <summary> <inheritdoc cref = "add_ek"> </inheritdoc> </summary>
        public uint add_EK { get { return add_ek; } set { add_ek = value; } }
        /// <summary> 
        ///     Хранит максимально возможный уровень ресурсной постройки который может построить НЕ столица. <br/>
        ///     Это значит что столицы могут строить ресурсную постройку до упячки (20 lvl), <br/>
        ///     а все остальные деревни (не столицы) ограничены этим уровнем.
        /// </summary>
        private uint limit_lvl = 5;
        /// <summary> <inheritdoc cref = "limit_lvl"> </inheritdoc> </summary>
        public uint Limit_Lvl { get { return limit_lvl; } set { limit_lvl = value; } }
        /// <summary> Хранит полное (уникальное) название альянса в котором находится аккаунт. Если аккаунт "безпартийный", то поле содержит: "-". </summary>
        private string alliance_name = "-";
        /// <summary> <inheritdoc cref = "alliance_name"> </inheritdoc> </summary>
        public string Alliance_Name { get { return alliance_name; } set { alliance_name = value; } }
        /// <summary> Возвращает или задаёт бонус для пивоварни (Германцы). +% к силе атаки всех войск аккаунта. Он учитывается при расчёте битвы если запущен праздник <b>  </b>. </summary>
        public int Attack_Bonus = 0;
        /// <summary> Возвращает или задаёт информацию о статусе праздника (Германцы). <b> true </b> = праздник запущен, <b> false </b> = нет. </summary>
        public bool Holiday = false;
        
        /// <summary> Возвращает или задаёт ссылку на альянс, в котором состоит аккаунт. <br/> Список альянсов хранится в <b>AllianceList[n];</b> </summary>
        public TAlliance.TData LinkOnAlliance = null;

        /// <summary>
        ///     Метод вычисляет количество деревень которые может иметь аккаунт. <br/>
        ///     Если вычисленное значение больше чем текущее в <b>Limit_Village</b>, <br/>
        ///     метод присвоит новое значение в поле <b>Limit_Village</b>.
        /// </summary>
        /// <value> <b>Speed:</b> переключатель вариантов загрузки таблицы культуры. </value>
        private void Check_Limit_Village(SpeedOfServerForCulture Speed = SpeedOfServerForCulture.x3) {
            var List = Newtonsoft.Json.JsonConvert.DeserializeObject<string[,]>
                (File.ReadAllText("DATA_BASE/JSON/culture_for_new_village_TRAVIAN(x3).json"));
            for (int i = 0; i < List.GetLength(0); i++) { 
                if (ek >= Convert.ToUInt32(List[i, 1])) { Limit_Village = Convert.ToUInt32(List[i, 0]); } else break;
            } 
        }

        /// <summary> Метод создаёт аккаунт из файла <b>.json</b> и заполняет некоторые поля сам. </summary>
        /// <value>
        ///     <b>FolderAccount:</b> название папки с именем аккаунта в которой хранятся все файлы аккаунта. <br/>
        ///     <b>Nick_Name:</b> игровой ник аккаунта (может совпадать с названием папки или не совпадать). <br/>
        ///     <b>Folk_Name:</b> название выбранной нации аккаунта. <br/>
        /// </value>
        public void CreateAccount(string FolderAccount, string Nick_Name, Folk Folk_Name) {
            this.FolderAccount = FolderAccount; this.Nick_Name = Nick_Name; this.Folk_Name = Folk_Name;
        }

        /// <summary> Метод создаёт героя из файла <b>.json</b> и заполняет некоторые поля сам. </summary>
        /// <value> <b>Folk_Name:</b> название выбранной нации аккаунта. </value>
        public void CreateHero(Folk Folk_Name) {
            string path = "DATA_BASE/JSON/unit_settings/";
            string Name = $"Hero_set_{Folk_Name}.json"; string Name2 = $"Hero_Recovery(x3)_{Folk_Name}.json";

            Hero = Newtonsoft.Json.JsonConvert.DeserializeObject<THero>(File.ReadAllText(path + Name));
            Hero.Array_Hero_Recovery = Newtonsoft.Json.JsonConvert.DeserializeObject<double[,]>(File.ReadAllText(path + Name2));
            Hero.Array_Hero_LVL_Kill = Newtonsoft.Json.JsonConvert.DeserializeObject<int[,]>
                (File.ReadAllText("DATA_BASE/JSON/unit_settings/Hero_LVL.json"));
        }

        /// <summary>
        ///     Метод создаёт дефолтную деревню для нового аккаунта из шаблона <b>.json</b> и последующие путём заселения поселенцами. <br/><br/>
        ///     Если деревня захватывается, то её хозяин просто теряет свою деревню так: <br/>
        ///     - происходит копирование строки из VillageList потерявшего деревню в VullageList захватившего; <br/>
        ///     - происходит удаление деревни из VillageList у аккаунта который потерял деревню; <br/>
        ///     - некоторые поля обоих аккаунтов вычисляются заново с учётом потери/захвата. <br/>
        ///     Для обработки захвата вызов этого метода не требуется.
        /// </summary>
        /// <remarks>
        ///     <b> Village: </b> экземпляр класса "деревня", в который предварительно загружены свойства из json файла. <br/>
        ///     <b> Game: </b> самый главный объект бэкенда, содержит все остальные классы бэкенда. <br/>
        ///     <b> Languages: </b> объект хранящий массивы с локализацией. <br/>
        ///     <b> Коры_X, Коры_Y: </b> координаты деревни в системе координат мира Travian +/- с центром (0, 0) на карте. Значение по умолчанию для обоих пар: 123456789. <br/>
        ///     Если координаты определены заранее, то деревня будет создана в указанных координатах, если координаты не переданы - деревня появится в случайном свободном месте.
        /// </remarks>
        public void CreateVillage(TVillage Village, TGame Game, WindowsInterface.Form1.TLANGUAGES Languages, 
                                    int Коры_X = 123456789, int Коры_Y = 123456789)
        {
            Point Location = new Point(Коры_X, Коры_Y); var Default = new Point(123456789, 123456789);

            //Выбор уникального имени для деревни ("Новая деревня 1", "Новая деревня 2" и т.д.)
            if (VillageList.Count <= 0) Village.Village_Name = Languages.RESOURSES[90] + " 1";
            else { int j = 0; while (true) { j++; bool b = false; string NewName = Languages.RESOURSES[90] + " " + j;
                for (int k = 0; k < VillageList.Count; k++) { 
                    if (NewName == VillageList[k].Village_Name) { b = false; break; } b = true;
                } if (b) { Village.Village_Name = NewName; break; }
            }}

            if (Location != Default) { //координаты для стартовой деревни определены заранее
                var ID = Game.Map.Cell[Location.X + Game.Map.Width, Location.Y + Game.Map.Height].ID;
                if (ID != Cell_ID.Null && ID != Cell_ID.Wild_Field) { MessageBox.Show("Error 22. TPlayer.CreateVillage(...);\nПереданные координаты ведут к занятой ячейке.\nMap.Cell[x, y].ID = " + ID + "\nВыход из приложения."); Environment.Exit(1); }
                Village.Coordinates_World_Travian = new Point(Location.X, Location.Y);
                Village.Coordinates_Cell = Game.Map.Coord_WorldToMap(Location);
                Game.Map.Cell[Location.X + Game.Map.Width, Location.Y + Game.Map.Height].ID = Cell_ID.Village_Tiny;//чтобы в генерации карты пропустить ячейку
            }
            else { while (true) { //генерировать до тех пор пока не найду пригодные коры для основания деревни
                int x = Random.RND(-Game.Map.Width, Game.Map.Width);
                int y = Random.RND(-Game.Map.Height, Game.Map.Height);
                if (x == 0 && y == 0) continue;//в координатах (0, 0) селиться нельзя. эти координаты для тестов
                if (Game.Map.Cell[x + Game.Map.Width, y + Game.Map.Height].ID == Cell_ID.Null ||
                    Game.Map.Cell[x + Game.Map.Width, y + Game.Map.Height].ID == Cell_ID.Wild_Field) {
                        Village.Coordinates_World_Travian = new Point(x, y);
                        Village.Coordinates_Cell = Game.Map.Coord_WorldToMap(x, y);
                        Game.Map.Cell[x + Game.Map.Width, y + Game.Map.Height].ID = Cell_ID.Village_Tiny;//чтобы в генерации карты пропустить ячейку
                        break;
            }}}

            if (Folk_Name == Folk.Rome) { Village.Slot[18].ID = Buildings.Городская_стена_Римляне; }
            else if (Folk_Name == Folk.German) { Village.Slot[18].ID = Buildings.Земляной_вал_Германцы; }
            else if (Folk_Name == Folk.Gaul) { Village.Slot[18].ID = Buildings.Изгородь_Галлы; }

            Village.Capacity = Game.Capacity_Basic;

            Village.OasisList.Clear();
            Village.Reinforcements.Clear();
            VillageList.Add(Village);//добавление деревни в лист деревень
            Game.Map.addAccountOnMap(this, Village, Game);//генерация карты следует позже и в ней сохраняются заполненные поля этой ячейки
        }

        /// <summary>
        ///     Метод загружает деревню из бинарного файла <b> Village[N].DAT </b> <br/>
        ///     Все значения бинарного файла заполняют соответствующие поля <b> Village </b> и <b> Village.Slot[] </b>. <br/>
        ///     Сформированная деревня <b> Village </b> добавляется в список деревень <b> VillageList </b>. <br/> <br/>
        ///     <b> <paramref name="Full_Path_save"/>: </b> полный путь к папке с деревней. <br/>
        ///     <b> <paramref name="Game"/>: </b> объект, у которого хранятся свойства войск, деревень, стек событий и т.д. <br/>
        /// </summary>
        public void LoadVillage(string Full_Path_save, TGame Game) {
            using (FileStream fs = new FileStream(Full_Path_save, FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                    var Village = new TVillage {
                        Village_Name = br.ReadString(),/*1*/
                        Type_Resources = (TypeCell)br.ReadInt32(),/*2*/Population = br.ReadInt32(),/*3*/
                        Approval = br.ReadInt32(),               /*4*/Add_Approval = br.ReadUInt32(),/*5*/
                        Culture_EK = br.ReadUInt32(),            /*6*/Add_Culture_EK = br.ReadUInt32(),/*7*/
                        Coordinates_World_Travian = new Point(br.ReadInt32(),/*8*/ br.ReadInt32()/*9*/),
                        Coordinates_Cell = new Point(br.ReadInt32(),/*8*/ br.ReadInt32()/*9*/),
                        HourlyProductionResources = new Res(br.ReadDouble(),/*10*/ br.ReadDouble(),/*11*/ br.ReadDouble(),/*12*/
                                                           br.ReadDouble(),/*13*/ br.ReadDouble()/*14*/),
                        Capacity = new sCapacity(br.ReadInt32()/*15*/, br.ReadInt32()/*16*/,
                                                   br.ReadInt32()/*17*/,    br.ReadInt32()/*18*/, br.ReadInt32()/*19*/),
                        ResourcesInStorages = new Res(br.ReadDouble(),/*20*/ br.ReadDouble(),/*21*/ br.ReadDouble(),/*22*/
                                                        br.ReadDouble(),/*23*/ br.ReadDouble()/*24*/),
                        Crop_Consumption = br.ReadInt32(),/*25*/ QuantityOfTraps_Total = br.ReadInt32(),/*26*/
                        QuantityOfTraps_Available = br.ReadInt32(),/*27*/ QuantityOfTraps_Built = br.ReadInt32(),/*28*/
                        QuantityMerchants_Available = br.ReadInt32(),/*29*/ QuantityMerchants_Total = br.ReadInt32(),/*30*/
                        HourlyProductionResources_PercentOfIncrease = new Res(br.ReadDouble(),/*31*/ br.ReadDouble(),/*32*/
                                                     br.ReadDouble(),/*33*/ br.ReadDouble(),/*34*/ br.ReadDouble()/*35*/),
                        Wounded_Units = new int[] { br.ReadInt32(),/*36*/ br.ReadInt32(),/*37*/ br.ReadInt32(),/*38*/
                                                     br.ReadInt32(),/*39*/ br.ReadInt32(),/*40*/ br.ReadInt32(),/*41*/
                                                     br.ReadInt32(),/*42*/ br.ReadInt32(),/*43*/ br.ReadInt32(),/*44*/
                                                     br.ReadInt32(),/*45*/ },
                        Wounded_TreatmentCostMultiplier = br.ReadDouble(),/*46*/ Wounded_DeathOfUnitsPerDay_Percent = br.ReadDouble(),/*47*/
                        BonusOfSpeed_Arena = br.ReadInt32(),/*48*/ BonusOfTime_Construction = br.ReadInt32(),/*49*/
                        QuantityOfVisibleMovements_CollectionPoint = br.ReadInt32(),/*50*/ BonusOfTimeTraining_Barrack = br.ReadInt32(),/*51*/
                        BonusOfTimeTraining_Stable = br.ReadInt32(),/*52*/ 
                        BonusOfTime_TrainingInWorkshop = br.ReadInt32(),/*53*/
                        ExpansionSlots_Free = br.ReadInt32(),/*54*/ ExpansionSlots_Occupied = br.ReadInt32(),/*55*/
                        BonusOfCapacity_Merchants = br.ReadInt32(),/*56*/
                        BonusOfTimeTraining_BigBarrack = br.ReadInt32(),/*57*/
                        BonusOfTimeTraining_BigStable = br.ReadInt32(),/*58*/
                        BonusOfProtection_Wall = br.ReadInt32(),/*59*/ 
                        CompleteDestruction_Wall_CountOfRams = br.ReadInt32(),/*60*/ 
                        BonusOfStability_Buildings = br.ReadInt32(),/*61*/
                        OasisSlots_Free = br.ReadInt32(),/*62*/ OasisSlots_Occupied = br.ReadInt32(),/*63*/
                        BonusOfTimeTraining_WateringHole = br.ReadInt32(),/*64*/
                        Bonus_OfCropConsumption_ByMountedScouts_ROMANS = br.ReadInt32(),/*65*/
                        Bonus_OfCropConsumption_ByEmperorCavalry_ROMANS = br.ReadInt32(),/*66*/
                        Bonus_OfCropConsumption_By_CaesarCavalry_ROMANS = br.ReadInt32(),/*67*/
                        DurationOfHoliday_TownHall = br.ReadInt32(),/*68*/
                        DurationOfCelebration_TownHall = br.ReadInt32(),/*69*/
                        Timer_Holiday_TownHalls = br.ReadInt32(),/*70*/
                        Timer_Celebration_TownHalls = br.ReadInt32(),/*71*/
                        Type_Village = (TypeVillage)br.ReadInt32(),/*74*/
                    };
                    //лист координат оазисов
                    int value = br.ReadInt32();/*75*/
                    if (value >= 1) for (int i = 0; i < value; i++) {
                        Village.OasisList.Add(new Point(br.ReadInt32(), br.ReadInt32()));
                    }

                    //НАВЕДИ КУРСОР НА TVillage И ПРОЧТИ !!!!!
                    //инициализация полей и построек
                    for (int i = 0; i < 18 + 22; i++) { 
                        Village.Slot[i] = new TVillage.TSlot {
                            Level = br.ReadInt32(), 
                            ID = (Buildings)br.ReadInt32(),
                            ProcessOfConstruction = br.ReadBoolean(),
                            ID_ProcessOfConstruction = (Buildings)br.ReadInt32(),
                        }; 
                    }
                    //загрузка листа подкреплений В КОНЦЕ ФАЙЛА, Т.К. СОХРАНЕНИЕ ПРОИСХОДИТ ТОЖЕ В КОНЕЦ ФАЙЛА
                    Village.Load_Reinforcements_Of_Troops(br);

                    VillageList.Add(Village);//добавление деревни в лист деревень
                    Game.Map.addAccountOnMap(this, Village, Game);//генерация карты следует позже и в ней сохраняются заполненные поля этой ячейки
            }}
        }

        /// <summary> 
        ///     Метод загружает конкретный аккаунт из папки с аккаунтами и все его деревни. <br/> <br/>
        ///     <b> <paramref name="NameFolderAccount"/>: </b> имя папки в которой хранятся все файлы аккаунта. <br/>
        ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
        ///     <b> <paramref name="Game"/>: </b> объект, у которого хранятся свойства войск, деревень, стек событий и т.д. <br/>
        /// </summary>
        /// <returns> Возвращает <b>true</b> если загрузка прошла корректно. В противном случае <b>false</b> и в месте вызова метода осуществляется выход из программы. </returns>
        public bool LoadAccount(string NameFolderAccount, string path, TGame Game) { 
            FolderAccount = NameFolderAccount;
            using (FileStream fs = new FileStream($"{path}/account.DAT", FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                    Nick_Name = br.ReadString();      /*0*/  Rank = br.ReadInt32();           /*1*/
                    Folk_Name = (Folk)br.ReadUInt32();/*2*/  Limit_Village = br.ReadUInt32();  /*3*/
                    ActiveIndex = br.ReadInt32();     /*4*/  Total_Population = br.ReadInt32();/*5*/
                    NumberOfCapital = br.ReadUInt32();/*6*/  EK = br.ReadUInt32();             /*7*/
                    add_EK = br.ReadUInt32();         /*8*/  Limit_Lvl = br.ReadUInt32();      /*9*/
                    Alliance_Name = br.ReadString();  /*10*/ Attack_Bonus = br.ReadInt32();    /*11*/
                    Holiday = br.ReadBoolean();/*12*/
            }}

            //загружаем все деревни аккаунта
            VillageList.Clear();
            string[] PathVillages = Directory.GetFiles(path + "/");//список путей к каждой деревне аккаунта
            if (PathVillages == null) return false;
            for (int i = 0; i < PathVillages.Length; i++) {
                if (PathVillages[i].Contains("Village")) LoadVillage(PathVillages[i], Game);
            }
            return true;
        }

        /// <summary> 
        ///     Метод сохраняет поля класса <b>TPlayer</b> в файлы: <b>account.DAT</b>, <b>Interface.DAT</b> <br/> <br/>
        ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
        /// </summary>
        public void SaveAccount(string path) {
            Directory.CreateDirectory(path);//создать папку с названием ник нэйма
            using (FileStream fs = new FileStream($"{path}/account.DAT", FileMode.Create)) {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                    bw.Write(Nick_Name);      /*0*/   bw.Write(Rank);           /*1*/
                    bw.Write((int)Folk_Name); /*2*/   bw.Write(Limit_Village);  /*3*/
                    bw.Write(ActiveIndex);    /*4*/   bw.Write(Total_Population);/*5*/
                    bw.Write(NumberOfCapital);/*6*/   bw.Write(EK);              /*7*/
                    bw.Write(add_EK);         /*8*/   bw.Write(Limit_Lvl);       /*9*/
                    bw.Write(Alliance_Name);  /*10*/  bw.Write(Attack_Bonus);    /*11*/
                    bw.Write(Holiday);        /*12*/
            }}
        }

        /// <summary> Метод проверяет имя альянса аккаунта и ссылку на него. </summary>
        /// <returns> Возвращает <b>true</b> если аккаунт состоит в альянсе и <b>false</b> - если не состоит. </returns>
        public bool IsAlliance() {
            return LinkOnAlliance != null && Alliance_Name != "" && Alliance_Name != "-" && Alliance_Name != "[]" && Alliance_Name != "[-]";
        }
        /// <summary> Метод осуществляет логику "светофора". <br/> Находится ли игрок онлайн или отсутствует. </summary>
        /// <returns> Возвращает <b>true</b> если игрок в игре (онлайн) и <b>false</b>, Если игрок не в игре (оффлайн). </returns>
        public bool IsOnline() {
            return true;
        }

        /// <summary> Метод удаляет деревню аккаунта по его индексу и перенаправляет его в столицу, которую невозможно уничтожить. </summary>
        /// <value>
        ///     <b><paramref name="index"/>:</b> номер деревни в листе деревень <b>VillageList</b>. <br/>
        ///     <b><paramref name="Map"/>:</b> ссылка на карту. <br/>
        /// </value>
        public void DeleteVillage(int index, TMap Map) {
            Map.Cell[VillageList[index].Coordinates_Cell.X, VillageList[index].Coordinates_Cell.Y].LinkAccount = null;
            Map.Cell[VillageList[index].Coordinates_Cell.X, VillageList[index].Coordinates_Cell.Y].LinkVillage = null;
            VillageList.RemoveAt(index);
            ActiveIndex_set_Village("");//переход к столице после удаления деревни, столицу уничтожить нельзя!
        }
        /// <summary> Метод удаляет аккаунт по его индексу и запрещает удалять игрока Player. </summary>
        /// <value>
        ///     <b><paramref name="index"/>:</b> номер аккаунта в листе ботов <b>BotList</b>. <br/>
        ///     <b><paramref name="BotList"/>:</b> ссылка на лист ботов. <br/>
        /// </value>
        public void DeleteAccount(int index, List<TPlayer> BotList) {
            if (BotList[index].Nick_Name != Nick_Name) {
                //for (int i = 0; i < BotList[index].VillageList.Count; i++) DeleteVillage(i, Map);//вроде как избыточно
                BotList.RemoveAt(index);
            }
        }
    }
}}