using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using static GameLogica.Enums_and_structs;

namespace GameLogica {
    public partial class TGame {
        /// <summary> содержит в себе необходимые поля и методы для реализации создания и хранения сообщений для вкладки "Сообщения". </summary>
        public class TMessage {
            /// <summary>
            ///     Поле <b> LIST </b> хранит все сообщения в каждой строке. <br/>
            ///     <b> TData </b> - общий подкласс для всех сообщений. <br/>
            ///     В каждой строке описан текст сообщения, отправитель/получатель сообщения (по координатам), заголовок сообщения, дата и т.д.
            /// </summary>
            public List<TData> LIST = new List<TData>();

            /// <summary>
            ///     Метод добавляет строки в лист сообщений <b>List[TData] Message</b>. <br/> Каждая строка содержит сообщение. <br/><br/>
            ///     Значение: <br/>
            ///     <b> <paramref name="TypeMessage"/>: </b> <inheritdoc cref="TData.TypeMessage"/> <br/>
            ///     <b> <paramref name="Cell_Start"/>: </b> <inheritdoc cref="TData.Cell_Start"/> <br/>
            ///     <b> <paramref name="Cell_Finish"/>: </b> <inheritdoc cref="TData.Cell_Finish"/> <br/>
            ///     <b> <paramref name="Depth"/>: </b> <inheritdoc cref="TData.Depth"/> <br/>
            ///     <b> <paramref name="Archive"/>: </b> <inheritdoc cref="TData.Archive"/> <br/>
            ///     <b> <paramref name="Read"/>: </b> <inheritdoc cref="TData.Read"/> <br/>
            ///     <b> <paramref name="Date"/>: </b> <inheritdoc cref="TData.Date"/> <br/>
            ///     <b> <paramref name="Time"/>: </b> <inheritdoc cref="TData.Time"/> <br/>
            ///     <b> <paramref name="Topic"/>: </b> <inheritdoc cref="TData.Topic"/> <br/>
            ///     <b> <paramref name="Text"/>: </b> <inheritdoc cref="TData.Text"/> <br/>
            /// </summary>
            /// <remarks> Пример кода для добавления строк сообщений в лист <b>Message</b> можно посмотреть в TGame.TEvent_Stack.Add(...); Метод заполняется аналогично. </remarks>
            /// <returns> Возвращает <b> true </b> если добавление прошло успешно. </returns>
            public bool Add(Type_Message TypeMessage, Point Cell_Start, Point Cell_Finish, int Depth, bool Archive, bool Read,
                string Date, string Time, string Topic, string Text) {
                LIST.Add(new TData(TypeMessage, Cell_Start, Cell_Finish, Depth, Archive, Read, Date, Time, Topic, Text));
                return true;
            }

            /// <summary>
            ///     Метод сохраняет содержимое всех строк поля TMessage.LIST в файл: <b>Messages.DAT</b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void SaveMessages(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Messages.DAT", FileMode.Create)) {
                    using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Default)) {
                        bw.Write(LIST.Count);/*0*/ //сохраняем количество строк в листе сообщений первым символом
                        if (LIST.Count <= 0) return;
                        for (int i = 0; i < LIST.Count; i++) {
                            bw.Write((int)LIST[i].TypeMessage);
                            bw.Write(LIST[i].Cell_Start.X);   bw.Write(LIST[i].Cell_Start.Y);
                            bw.Write(LIST[i].Cell_Finish.X);  bw.Write(LIST[i].Cell_Finish.Y);
                            bw.Write(LIST[i].Depth);
                            bw.Write(LIST[i].Archive);        bw.Write(LIST[i].Read);
                            bw.Write(LIST[i].Date);           bw.Write(LIST[i].Time);
                            bw.Write(LIST[i].Topic);          bw.Write(LIST[i].Text);

                            LoadProcess.LoadText.Text = Text + $" Message[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary>
            ///     Метод загружает в лист сообщений <b>TMessages.LIST</b> данные из бинарного файла <b> Messages.DAT </b> <br/> Метод содержит в своём теле цикла инкремент: <b>LoadProcess._Update();</b> <br/> <br/>
            ///     <b> <paramref name="path"/>: </b> <inheritdoc cref="PathFolderSave"/> ДО НАЗВАНИЯ АККАУНТА! <br/>
            /// </summary>
            public void LoadMessages(UFO.TLoadProcess LoadProcess, string path) {
                string Text = LoadProcess.LoadText.Text;
                using (FileStream fs = new FileStream($"{path}/Messages.DAT", FileMode.Open)) {
                    using (BinaryReader br = new BinaryReader(fs, Encoding.Default)) {
                        int Messages_Count = br.ReadInt32();/*0*/ if (Messages_Count <= 0) return;
                        for (int i = 0; i < Messages_Count; i++) {
                            var Data = new TData {
                                TypeMessage = (Type_Message)br.ReadInt32(),
                                Cell_Start = new Point(br.ReadInt32(), br.ReadInt32()),
                                Cell_Finish = new Point(br.ReadInt32(), br.ReadInt32()),
                                Depth = br.ReadInt32(),
                                Archive = br.ReadBoolean(),  Read = br.ReadBoolean(),
                                Date = br.ReadString(),      Time = br.ReadString(),
                                Topic = br.ReadString(),     Text = br.ReadString(),
                            };
                            LIST.Add(Data);
                            LoadProcess.LoadText.Text = Text + $" Message[{i}]"; LoadProcess._Update();
                        }
                    }
                }
            }

            /// <summary> хранит всю необходимую информацию о сообщении. Является строкой в листе <b>Message.LIST[TData];</b> </summary>
            /// <remarks> 
            ///     Доступ к полям осуществляется так: <br/>
            ///     <b> Message.LIST[N].Date = "16.11.2022 18:30:17"; </b> <br/>
            ///     <b> Message.LIST[N].Text = "Гутен утер, либе муттер. Шрайбт письмишко Димка твой: Битте, битте, либе муттер, Приготовь мне брот унд буттер С баклажанною икрой."; </b> <br/>
            ///     <b> Message.Add(...); </b> <br/><br/>
            /// </remarks>
            public class TData {
                /// <summary> Тип сообщения. По умолчанию <b>TypeMessage</b> = Incoming; (входящее) </summary>
                public Type_Message TypeMessage = 0;
                /// <summary> Хранит координаты ячейки которая является отправителем сообщения в системе координат Cell[x][y]. </summary>
                public Point Cell_Start = new Point(0, 0);
                /// <summary> Хранит координаты ячейки которая является адресатом сообщения в системе координат Cell[x][y]. </summary>
                public Point Cell_Finish = new Point(0, 0);
                /// <summary> Глубина переписки. -1 = topic; 0 = RE: topic; 1 = RE^1: topic; 2 = RE^2: topic; 3..n = RE^3..n topic. </summary>
                public int Depth = -1;
                /// <summary> Дата сгенерированного сообщения. Тип: ДД.ММ.ГГ. </summary>
                public string Date = "";
                /// <summary> Время сгенерированного сообщения. Тип: ч:м:с. </summary>
                public string Time = "";
                /// <summary> Тема сообщения для вкладки "Сообщения". </summary>
                public string Topic = "";
                /// <summary> Текст сообщения для вкладки "Сообщения". </summary>
                public string Text = "";
                /// <summary> Флаг статуса сообщения. true = сообщение помещёно в архив, false = сообщение не в архиве. </summary>
                public bool Archive = false;
                /// <summary> Флаг статуса сообщения. true = сообщение прочитано, false = сообщение не прочитано. </summary>
                public bool Read = false;

                public TData() { }
                public TData(Type_Message TypeMessage, Point Cell_Start, Point Cell_Finish, int Depth, bool Archive, bool Read,
                    string Date, string Time, string Topic, string Text) {
                    this.TypeMessage = TypeMessage;  this.Cell_Start = Cell_Start;  this.Cell_Finish = Cell_Finish;
                    this.Depth = Depth;              this.Archive = Archive;        this.Read = Read;
                    this.Date = Date;                this.Time = Time;              this.Topic = Topic;
                    this.Text = Text;                
                }

                /// <summary> Метод получает приставку вида <b>"RE^n:"</b> к заголовку темы. </summary>
                /// <remarks> Возможные варианты: <br/> При <b>Depth = -1</b> return ""; <br/> При <b>Depth = 0</b> return "RE:"; <br/> При <b>Depth = 1</b> return "RE^1:"; <br/> При <b>Depth = 2</b> return "RE^2:"; <br/> При <b>Depth = 3</b> return "RE^3:"; <br/> При <b>Depth = 4</b> return "RE^4:"; <br/> и т.д. </remarks>
                /// <returns> Возвращает приставку вида <b>"RE^n:"</b> к заголовку темы. </returns>
                public string GetRE(int Depth) { return Depth <= -1 ? "" : Depth == 0 ? "RE:" : $"RE^{Depth}:"; }
                /// <summary> <inheritdoc cref="GetRE"/> </summary> <remarks> <inheritdoc cref="GetRE"/> </remarks> <returns> <inheritdoc cref="GetRE"/> </returns>
                public string GetRE() { return GetRE(Depth); }
            }
        }
        /// <summary> Объект сообщений. Хранит список всех сообщений связанных с игроком. </summary>
        public TMessage Messages = null;
    }
}
