using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using static GameLogica.Enums_and_structs;

namespace GameLogica {
public partial class TGame {
    public partial class TPlayer {
        public partial class TVillage {
            /// <summary> реализует хранение войск в листах <b>исход./приход.</b> и всё что с ними связано для пункта сбора деревни. <br/> В листах хранятся только переданные подкрепы. Войска в пути для пункта сбора берутся из стека событий. </summary>
            public class Reinforcements_Of_Troops {
                /// <summary>
                ///     Хрранит ссылку на аккаунт владеющий стеком войск. <br/>
                ///     Зацепка ссылки происходит во время загрузки игры.
                /// </summary>
                public TPlayer LinkAccount = null;
                /// <summary> 
                ///     Хранит ссылку на деревню владеющей стеком войск. <br/>
                ///     Зацепка ссылки происходит во время загрузки игры.
                /// </summary>
                public TVillage LinkVillage = null;
                /// <summary>
                ///     Хранит ключ (хэш) строки. Уникальный идентификационный номер ID между двумя деревнями. <br/>
                ///     По этому ключу находится подкреп во второй деревне. <br/> Сравнение ключей заменяет сравнение величин массивов Units, наций, владельцев подкрепа и т.д.
                /// </summary>
                public string Key;
                /// <summary> Хранит тип подкрепления: Входящее/Исходящее. <br/> Этот флаг определяет кто кому прислал подкрепление посредством сохранения разных координат в списки 2 деревень: (получатель, отправитель) </summary>
                public Подкреп Подкрепление;
                /// <summary> Название нации подкрепления. </summary>
                public Folk FolkName;
                /// <summary> Хранит координаты на ячейку, которая владеет стеком войск. </summary>
                public Point CoordCell;
                /// <summary> Хранит массив переданных войск на [11] элементов. Подкрепление. </summary>
                /// <remarks> [0..9] - войска, [10] - герой. </remarks>
                public int[] Units = new int[11];

                /// <summary> Конструктор добавляет поля своего класса. </summary>
                public Reinforcements_Of_Troops(string Key, Подкреп Подкрепление, Folk FolkName,
                                                Point CoordCell, int[] Юниты) {
                    this.Key = Key;              this.Подкрепление = Подкрепление;
                    this.FolkName = FolkName;    this.CoordCell = CoordCell;
                    Units = Юниты;
                }
            }
            /// <summary>
            ///     Список хранит все входящие/исходящие подкрепления. <br/>
            ///         Если в списке <b>Подкреп.Вход</b> - то в <b>CoordCell</b> хранятся координаты того кто ПРИСЛАЛ подкреп. <br/>
            ///         Если в списке <b>Подкреп.Исход</b> - то в <b>CoordCell</b> хранятся координаты того кто ОТПРАВИЛ подкреп. <br/>
            ///     А так же в списке хранятся все пойманные войска капканщиком галлов. <br/>
            ///         Если в списке <b>Подкреп.Вход</b> - то в <b>CoordCell</b> хранятся координаты деревни, чьи войска сели в капканы. <br/>
            ///         Если в списке <b>Подкреп.Исход</b> - то в <b>CoordCell</b> хранятся координаты деревни, которая морит присланные войска в капканах. <br/>
            ///     <br/> Используется для пункта сбора. 
            /// </summary>
            public List<Reinforcements_Of_Troops> Reinforcements = new List<Reinforcements_Of_Troops>();


            /// <summary>
            ///     Метод сохраняет листы <b>Incoming_Reinforcements, Outgoing_Reinforcements</b> в файл: <b>Village[N].DAT</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> + название папки аккаунта. <br/>
            /// </summary>
            public void Save_Reinforcements_Of_Troops(string path) {
                using (FileStream fs = new FileStream($"{path}/Village [" + Village_Name + "].DAT", FileMode.Append)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        var _ = Reinforcements;
                        if (_.Count <= 0) bw.Write(-1);//-1 = отсутствие списка, >=1 = Count
                        else { bw.Write(_.Count);
                            for (int i = 0; i < _.Count; i++ ) {
                                bw.Write(_[i].Key);            bw.Write((int)_[i].Подкрепление);
                                bw.Write((int)_[i].FolkName);    
                                bw.Write(_[i].CoordCell.X);    bw.Write(_[i].CoordCell.Y);
                                for (int n = 0; n < _[i].Units.Length; n++) { bw.Write(_[i].Units[n]); }
                            }
                        } 
                    }
                }
            }

            /// <summary> Метод загружает лист <b>Reinforcements</b> из бинарного файла <b> Village[N].DAT </b> </summary>
            /// <remarks>
            ///     <b> br: </b> файл <b> Village[N].DAT </b> открытый на чтение. <br/>
            /// </remarks>
            public void Load_Reinforcements_Of_Troops(BinaryReader br) {
                int value = br.ReadInt32();
                if (value >= 1) for (int i = 0; i < value; i++) {
                    Reinforcements.Add( new Reinforcements_Of_Troops(
                        br.ReadString(), (Подкреп)br.ReadInt32(), (Folk)br.ReadInt32(),
                        new Point(br.ReadInt32(), br.ReadInt32()),
                        new int[] { br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), 
                            br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32(),
                            br.ReadInt32(), br.ReadInt32(), br.ReadInt32() }));
                }
            }

            /// <summary> Метод добавляет строки с подкреплениями в List. </summary>
            /// <value>
            ///     <b> <paramref name="Key"/>: </b> уникальный идентификационный ключ/номер строки [ID]. <br/>
            ///     <b> <paramref name="Подкрепление"/>: </b> тип подкрепления: Вход, Исход. <br/>
            ///     <b> <paramref name="FolkName"/>: </b> название нации подкрепления. <br/>
            ///     <b> <paramref name="CoordCell"/>: </b> координаты на ячейку, которая владеет стеком войск. <br/>
            ///     <b> <paramref name="Юниты"/>: </b> массив переданных войск на [11] элементов. Подкрепление. [0..9] - войска, [10] - герой <br/>
            /// </value>
            public void Add_Reinforcement(string Key, Подкреп Подкрепление, Folk FolkName, Point CoordCell, int[] Юниты) {
                int[] CopyArray = new int[Юниты.Length]; Юниты.CopyTo(CopyArray, 0);
                Reinforcements.Add(new Reinforcements_Of_Troops(Key, Подкрепление, FolkName, CoordCell, CopyArray));
            }

            /// <summary> Метод сравнивает переданный ключ <b>key</b> с листами <b>left</b> и <b>right</b> на предмет уникальности. </summary>
            /// <value>
            ///     <b> <paramref name="key"/>: </b> ключ на проверку уникальности. <br/>
            ///     <b> <paramref name="left"/>: </b> первый лист <b>Incoming_Reinforcements</b>. <br/>
            ///     <b> <paramref name="right"/>: </b> второй лист <b>Outgoing_Reinforcements</b>. <br/>
            /// </value>
            /// <returns></returns>
            private bool IsUniqueKey(string key,
                         List<Reinforcements_Of_Troops> left, List<Reinforcements_Of_Troops> right) {
                if (left.Count <= 0 && right.Count <= 0) return true;//ключ уникальный
                for (int l = 0; l < left.Count; l++) if (left[l].Key == key) return false;//есть такой ключ
                for (int r = 0; r < right.Count; r++) if (right[r].Key == key) return false;//есть такой ключ
                return true;//ключ не найден значит он уникальный
            }
            /// <summary> Метод создаёт уникальный ключ для списков обоих аккаунтов с помощью <b>DateTime</b>. </summary>
            /// <returns> Возвращает string число. Уникальный Key/ID для обоих аккаунтов. </returns>
            public string CreateKey_DateTime(List<Reinforcements_Of_Troops> left, List<Reinforcements_Of_Troops> right) {
                string key = DateTime.Now.Ticks.ToString();//генерация ключа
                while (true) {
                    if (IsUniqueKey(key, left, right)) return key;//проверка на уникальность в 2 списках
                    else { long inc = Convert.ToInt32(key); inc++; key = inc.ToString(); }//инкремент ключа
                }
            }

            /// <summary> Метод создаёт уникальный ключ для списков обоих аккаунтов с помощью структуры <b>Guid</b>. </summary>
            /// <returns> Возвращает string число. Уникальный Key/ID для обоих аккаунтов. </returns>
            public string CreateKey_Hash(List<Reinforcements_Of_Troops> left, List<Reinforcements_Of_Troops> right) {
                while (true) {
                    string key = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(20);
                    if (IsUniqueKey(key, left, right)) return key;//проверка на уникальность в 2 списках
                }
            }
        }
    }
}
}