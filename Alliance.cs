using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameLogica {
    public partial class TGame {
        /// <summary> содержит поля и методы для реализации альянса, в котором состоит игрок. </summary>
        public class TAlliance {
            /// <summary> 
            ///     Поле <b> LIST </b> хранит все альянсы в каждой строке. <br/>
            ///     <b> TData </b> - общий подкласс для всех альянсов. <br/>
            ///     В каждой строке описаны свойства каждого альянса: название, владелец, лист координат столиц участников и т.д.
            /// </summary>
            public List<TData> LIST = new List<TData>();

            /// <summary>
            ///     Метод добавляет строки в лист альянсов <b>List[TData] Alliance</b>. <br/> Каждая строка содержит альянс. <br/><br/>
            ///     Значение: <br/>
            ///     <b> <paramref name="AllianceNameAbbreviated"/>: </b> <inheritdoc cref="TData.AllianceNameAbbreviated"/> <br/>
            ///     <b> <paramref name="AllianceNameFull"/>: </b> <inheritdoc cref="TData.AllianceNameFull"/> <br/>
            ///     <b> <paramref name="Owner"/>: </b> <inheritdoc cref="TData.Owner"/> <br/>
            ///     ____________________________________________________________________________ <br/>
            ///     Эти поля заполняются: <br/>
            ///     <b> <paramref name="Rank"/>: </b> <inheritdoc cref="TData.Rank"/> <br/>
            ///     <b> <paramref name="TotalPopulation"/>: </b> <inheritdoc cref="TData.TotalPopulation"/> <br/>
            ///     Заполняются в <b>TData.AddNewMember(...); <inheritdoc cref="TData.AddNewMember"/></b>
            /// </summary>
            /// <remarks> Пример кода для добавления строк альянсов в лист <b>Alliance</b> можно посмотреть в TGame.TEvent_Stack.Add(...); Метод заполняется аналогично. </remarks>
            /// <returns> Возвращает <b> true </b> если добавление прошло успешно. </returns>
            public bool Add(string AllianceNameAbbreviated, string AllianceNameFull, Point Owner) {
                LIST.Add(new TData(AllianceNameAbbreviated, AllianceNameFull, Owner, -1, -1));
                return true;
            }

            /// <summary>
            ///     Метод сохраняет содержимое всех строк поля TAlliance.LIST в файл: <b>Alliances.DAT</b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void SaveAlliances(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Alliances.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(LIST.Count);/*0*/ //сохраняем количество строк в листе альянсов первым символом
                        if (LIST.Count <= 0) return;
                        for (int i = 0; i < LIST.Count; i++) {
                            bw.Write(LIST[i].AllianceNameAbbreviated);  bw.Write(LIST[i].AllianceNameFull);
                            bw.Write(LIST[i].Owner.X);                  bw.Write(LIST[i].Owner.Y);
                            bw.Write(LIST[i].Rank);                     bw.Write(LIST[i].TotalPopulation);

                            bw.Write(LIST[i].ListAlly.Count);//сохраняем кол-во строк в листе участников сохраняемого альянса
                            for (int j = 0; j < LIST[i].ListAlly.Count; j++) {
                                bw.Write(LIST[i].ListAlly[j].CoordinatesCell.X);
                                bw.Write(LIST[i].ListAlly[j].CoordinatesCell.Y);
                                bw.Write(LIST[i].ListAlly[j].DateTime_Add);
                                bw.Write(LIST[i].ListAlly[j].DateTime_Online);
                            }

                            LoadProcess.LoadText.Text = Text + $" Alliance[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary>
            ///     Метод загружает в лист альянсов <b>TAlliance.LIST</b> данные из бинарного файла <b> Alliances.DAT </b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void LoadAlliances(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Alliances.DAT", FileMode.Open)) {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                        int Alliance_Count = br.ReadInt32();/*0*/ if (Alliance_Count <= 0) return;
                        for (int i = 0; i < Alliance_Count; i++) {
                            //добавление альянса
                            var Data = new TData {
                                AllianceNameAbbreviated = br.ReadString(),          AllianceNameFull = br.ReadString(),
                                Owner = new Point(br.ReadInt32(), br.ReadInt32()),  Rank = br.ReadInt32(),
                                TotalPopulation = br.ReadInt32(),
                            };
                            int Ally_Count = br.ReadInt32();
                            for (int j = 0; j < Ally_Count; j++) {
                                //добавление свойств участников альянса
                                TData.Property prop = new TData.Property {
                                    CoordinatesCell = new Point(br.ReadInt32(), br.ReadInt32()),
                                    DateTime_Add = br.ReadInt64(), DateTime_Online = br.ReadInt64()
                                };
                                Data.ListAlly.Add(prop);
                            }

                            LIST.Add(Data);
                            LoadProcess.LoadText.Text = Text + $" Alliance[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary> хранит всю необходимую информацию об альянсе. Является строкой в листе <b>Alliance.LIST[TData];</b> </summary>
            public class TData {
                /// <summary> Дополнительные подробности об участнике альянса. <br/> Коры его столицы, дата и время вступления, дата и время последнего нахождения в игре. </summary>
                public struct Property {
                    /// <summary> Координаты столицы участника альянса в системе координат <b>Map.Cell[x][y]</b>. </summary>
                    public Point CoordinatesCell;
                    /// <summary> Бинарная Дата и время вступления в альянс 1010011. </summary>
                    public long DateTime_Add;
                    private long datetime_online;
                    /// <summary> Бинарная Дата и время последнего нахождения в игре участника альянса 1010011. </summary>
                    /// <remarks> 0 = онлайн; delta между текущей датой и временем формата <b>int</b> и этим полем, показывает как давно аккаунт не онлайн. <br/> Из этой длительности вычисляется какой кружок грузить в обзоре альянса: blue, green, yellow, red, gray. </remarks>
                    public long DateTime_Online { get { return datetime_online; } set { datetime_online = value; } }

                    public Property(Point CoordinatesCell, long DateTime_Add, long DateTime_Online) {
                        this.CoordinatesCell = CoordinatesCell; this.DateTime_Add = DateTime_Add;
                        datetime_online = DateTime_Online;
                    }
                    /// <summary> Метод обновляет бинарную дату и время последнего нахождения в игре участника альянса 1010011. </summary>
                    public void Refresh() { DateTime_Online = DateTime.Now.ToBinary(); }
                }

                /// <summary> Сокращённое название альянса. </summary>
                public string AllianceNameAbbreviated = "";
                /// <summary> Полное (уникальное) название альянса. </summary>
                public string AllianceNameFull = "";
                /// <summary> Координаты столицы владельца альянса (держателя посольства) в системе координат <b>Map.Cell[x][y];</b> </summary>
                public Point Owner = new Point(-1, -1);
                /// <summary> Возвращает или задаёт ранг альянса по суммарному населению участников для статистики. </summary>
                public int Rank = 0;
                /// <summary> Суммарное население участников альянса. </summary>
                public int TotalPopulation = 0;
                //public int AllianceCountAlly - кол-во участников альянса определяется по полю ListAlly.Count;
                /// <summary> Список подробной информации об участниках альянса (например координаты столиц). <br/> В списке так же находятся координаты основателя альянса. </summary>
                public List<Property> ListAlly = new List<Property>();

                public TData() { }
                public TData(string AllianceNameAbbreviated, string AllianceNameFull, Point Owner, int Rank, int TotalPopulation) {
                    this.AllianceNameAbbreviated = AllianceNameAbbreviated;  this.AllianceNameFull = AllianceNameFull;
                    this.Owner = Owner;                                      this.Rank = Rank;
                    this.TotalPopulation = TotalPopulation;
                }

                /// <summary> Метод добавляет нового участника в альянс. <br/> Поле <b>Rank</b> заполняется в методе: <b>Form1.SortRank_TotalPopulationAlliance();</b> </summary>
                public void AddNewMember(TPlayer Account) {
                    TotalPopulation += Account.Total_Population;
                    ListAlly.Add(new Property(Account.VillageList[0].Coordinates_Cell,
                                     DateTime.Now.ToBinary(), DateTime.Now.ToBinary()));
                    Account.Alliance_Name = AllianceNameFull;
                    Account.LinkOnAlliance = this;
                }
                /// <summary> Метод получает индекс в <b>ListAlly[i]</b>, которому соответствует аккаунт. </summary>
                /// <returns> Возвращает индекс в листе <b>ListAlly[i]</b>, номер строки в которой хранятся координаты столицы аккаунта. <br/> Если координаты не найдены, возвращает <b>-1.</b> </returns>
                public int GetIndex(TPlayer Account) {
                    for (int i = 0; i < ListAlly.Count; i++)
                        if (ListAlly[i].CoordinatesCell == Account.VillageList[0].Coordinates_Cell) return i;
                    return -1;
                }
                /// <summary> Метод получает структуру <b>Property</b> в листе <b>ListAlly[i]</b>, которой соответствует аккаунт. </summary>
                /// <returns> Возвращает структуру в листе <b>ListAlly[i]</b>, в которой хранятся координаты столицы аккаунта. <br/> Если координаты не найдены, возвращает <b>Property();</b> </returns>
                public Property GetProperty(TPlayer Account) {
                    for (int i = 0; i < ListAlly.Count; i++)
                        if (ListAlly[i].CoordinatesCell == Account.VillageList[0].Coordinates_Cell) return ListAlly[i];
                    return new Property();
                }
                /// <summary> Метод получает экземпляр <b>TData</b> в листе <b>Alliances.LIST[i]</b>, которому соответствует аккаунт. </summary>
                /// <returns> Возвращает экземпляр альянса в листе <b>Alliances.LIST[i]</b>, в котором хранятся координаты столицы аккаунта. <br/> Если координаты не найдены, возвращает <b>null;</b> </returns>
                public TData GetAlliance(TPlayer Account) {
                    for (int i = 0; i < ListAlly.Count; i++)
                        if (ListAlly[i].CoordinatesCell == Account.VillageList[0].Coordinates_Cell) return this;
                    return null;
                }
            }
        }
        /// <summary> Объект альянсов. Хранит список всех альянсов связанных с игрой. </summary>
        public TAlliance Alliances = null;
    }
}
