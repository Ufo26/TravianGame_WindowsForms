using GameLogica;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using static UFO.Convert;

namespace WindowsInterface {
    public partial class Form1 : Form {
        /// <summary> Таблица grid_Messages. Цвет текста 2..3 столбцов ячейки с не прочитанным сообщением. </summary>
        private Color grid_Messages_Cell_3_Color_Read_false = Color.FromArgb(135, 180, 30);
        /// <summary> Таблица grid_Messages. Цвет текста 2..3 столбцов ячейки с прочитанным сообщением. </summary>
        private Color grid_Messages_Cell_3_Color_Read_true = Color.FromArgb(135, 180, 30);

        /// <summary> Метод обрабатывает клик по содержимому ячеек таблицы <b>grid_Messages.</b> </summary>
        private void grid_Messages_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0) return; var grid = sender as DataGridView;
            var Cell = grid[1, e.RowIndex].Tag as TMap.TCell;
            if (e.ColumnIndex == 2) {
                WinDlg_Message(e.RowIndex, (TMessage.TData)((DataGridView)sender).Rows[e.RowIndex].Cells[0].Tag, Window_Message.Read);
            } else if (e.ColumnIndex == 3) {
                //открыть окно аккаунта отправителя сообщения
                if (Form_AccountProfile != null && Form_AccountProfile.Visible) Form_AccountProfile.Close();
                if (Cell.LinkAccount != null) WinDlg_AccountProfile(Cell.LinkAccount);
            }
            if (MessageIndex > -1) {
                var _ = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                if (e.ColumnIndex == 0) _.Value = !(bool)_.Value;//чекаем-не чекаем боксы
                _.Selected = false;//отмена выделения ячейки
            }
        }
        /// <summary> Метод обрабатывает наведение на ячейку <b>grid_Messages.</b> </summary>
        private void grid_Messages_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) if (e.ColumnIndex == 2 || e.ColumnIndex == 3) {
                ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки <b>grid_Messages.</b> </summary>
        private void grid_Messages_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) if (e.ColumnIndex == 2 || e.ColumnIndex == 3) {
                if (((TMessage.TData)((DataGridView)sender)[0, e.RowIndex].Tag).Read)
                    ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Style.ForeColor = grid_Messages_Cell_3_Color_Read_true;
                else ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Style.ForeColor = grid_Messages_Cell_3_Color_Read_false;
            }
        }

        /// <summary> Метод получает пиктограмму "прочитанного/не рпочитанного" сообщения. </summary>
        /// <value> 
        ///     <b> <paramref name="Read"/>: </b> флаг сообщения, <b>true</b> = сообщение прочитано, <b>false</b> = сообщение не прочитано. <br/>
        ///     <b> <paramref name="w"/>/<paramref name="h"/>: </b> ширина и высота картинки после преминения метода <b>Extensions.ResizeImage(...);</b> <br/>
        /// </value>
        /// <returns> Возвращает картинку пиктограммы "прочитанного/не рпочитанного" сообщения. <br/> Если картинка не найдена, метод пытается вернуть "null.png"; <br/> Если и её нет, то возвращает <b>null</b>. </returns>
        private Image getICO_Message(bool Read, int w = 19, int h = 16) {
            string path = "DATA_BASE/IMG/pictograms/ico";
            return Extensions.ResizeImage(File.Exists($"{path}/read_{Read}.png") ? Image.FromFile($"{path}/read_{Read}.png")
                : File.Exists($"{path}/null.png") ? Image.FromFile($"{path}/null.png") : null, ToCSR(w), ToCSR(h));
        }

        /// <summary> Метод помечает строку в таблице сообщений как прочитанное/не прочитанное сообщение. </summary>
        /// <value>
        ///     <b> <paramref name="Read"/>: </b> флаг сообщения, <b>true</b> = сообщение прочитано, <b>false</b> = сообщение не прочитано. <br/>
        ///     <b> <paramref name="Line"/>: </b> номер строки таблицы <b>grid_Messages</b>, в которой произойдут изменения. <br/>
        /// </value>
        public void Messages_ReadNotRead(bool Read, int Line) {
            //выход если выбран фильтр "отправленные". в отправленных прочитанность сообщения определяется тем кто прочитал, т.е. ботом
            //if ((int)btn_Messages_Outgoing.Tag == 1) return;
            FontStyle Font_Style; float FontSize; Color cl;
            if (Read) { cl = grid_Messages_Cell_3_Color_Read_false;
                Font_Style = FontStyle.Regular; FontSize = ToCSR(10);
                grid_Messages[1, Line].ToolTipText = LANGUAGES.Messages[11];/*Сообщение прочитано.*/  //всплывающая подсказка
            } else { cl = grid_Messages_Cell_3_Color_Read_true;
                Font_Style = FontStyle.Bold; FontSize = ToCSR(11);
                grid_Messages[1, Line].ToolTipText = LANGUAGES.Messages[12];/*Сообщение не прочитано.*/  //всплывающая подсказка
            }
            //форматирование ячеек таблицы
            grid_Messages[2, Line].Style.Font = grid_Messages[3, Line].Style.Font =
                new Font(grid_Messages.Columns[2].DefaultCellStyle.Font.FontFamily, FontSize, Font_Style);
            grid_Messages[2, Line].Style.ForeColor = grid_Messages[3, Line].Style.ForeColor = cl;
            //изменение картинки (пиктограммы)
            Image ico = getICO_Message(Read); if (ico == null) ico = new Bitmap(1, 1);
            ((DataGridViewImageCell)grid_Messages[1, Line]).Value = ico;
        }

        /// <summary> Метод обновляет вкладку "Сообщения". </summary>
        public void Update_Panels_Messages() {
            NewPositionButtons_pnl_Messages_Buttons();//положение кнопок на панели pnl_Messages_Buttons
            //лепим панели по центру
            pnl_Messages_Head.Centering_X(ToCSR(140));
            pnl_Messages_Buttons.Centering_X(pnl_Messages_Head.Top + pnl_Messages_Head.Height);
            grid_Messages.Centering_X(pnl_Messages_Buttons.Top + pnl_Messages_Buttons.Height + ToCSR(10));
            pnl_Messages_Footer.Centering_X(pnl_Messages_Footer.Parent.Height - pnl_Messages_Footer.Height - ToCSR(10));
            grid_Messages.Height = pnl_Messages_Footer.Top - grid_Messages.Top - ToCSR(10);
            NewPositionControls_pnl_Messages_Footer();//положение контролов на панели pnl_Messages_Footer
            grid_Messages.Rows.Clear(); chb_Messages_All.Checked = false;

            //если есть хотя бы одно сообщение
            if ((int)btn_Messages_Write.Tag != 1) if (GAME.Messages.LIST.Count > 0) {
                for (int i = GAME.Messages.LIST.Count - 1; i >= 0 ; i--) { //самое свежее сообщение появляется вверху таблицы
                    var LIST = GAME.Messages.LIST[i];
                    if ((int)btn_Messages_Archive.Tag == 1) { if (!LIST.Archive) continue; }//выбран фильтр АРХИВНЫЕ сообщения
                    else if((int)btn_Messages_Incoming.Tag == 1) //выбран фильтр ВХОДЯЩИЕ сообщения
                        { if (LIST.TypeMessage != Type_Message.Incoming || LIST.Archive) continue; }
                    else if ((int)btn_Messages_Outgoing.Tag == 1) //выбран фильтр ОТПРАВЛЕННЫЕ сообщения
                        { if (LIST.TypeMessage != Type_Message.Outgoing || LIST.Archive) continue; }

                    var Cell_Start = LIST.Cell_Start; var Cell_Finish = LIST.Cell_Finish;
                    var Start_LinkAccount = GAME.Map.Cell[Cell_Start.X, Cell_Start.Y].LinkAccount;
                    var Finish_LinkAccount = GAME.Map.Cell[Cell_Finish.X, Cell_Finish.Y].LinkAccount;
                    var add_NickName = (int)btn_Messages_Outgoing.Tag == 1 ? Finish_LinkAccount.Nick_Name : Start_LinkAccount.Nick_Name;
                    Image ico = getICO_Message(LIST.Read); if (ico == null) ico = new Bitmap(1, 1);
                    //добавление строки с данными
                    string DateTime = LIST.Date + " " + LANGUAGES.Reports[47]/*в*/ + " " + LIST.Time;
                    grid_Messages.Rows.Add(false, ico, $"{LIST.GetRE()} {LIST.Topic}", $"  {add_NickName}  ", $"  {DateTime}  ");

                    int push_back = grid_Messages.Rows.Count - 1;
                    if (ico == null) grid_Messages.Rows[push_back].Cells[1].ToolTipText = LANGUAGES.Reports[13];/*[null] Пиктограмма не найдена*/
                    grid_Messages[0, push_back].Tag = LIST;//ссылка на сообщение в LIST
                    grid_Messages[1, push_back].Tag = GAME.Map.Cell[Cell_Start.X, Cell_Start.Y];//ссылка на ячейку отправителя сообщения
                    //дополнительно редактируем заголовки прочитаных/не прочитанных сообщений
                    Messages_ReadNotRead(LIST.Read, push_back);
                }
            }

            //скрыть выделение ячеек
            grid_Messages.DefaultCellStyle.SelectionBackColor = grid_Messages.DefaultCellStyle.BackColor;
            grid_Messages.DefaultCellStyle.SelectionForeColor = grid_Messages.DefaultCellStyle.ForeColor;
            grid_Messages.Focus();
        }

        /// <summary> Метод вычисляет новую позицию кнопок-фильтров на панели pnl_Messages_Buttons. </summary>
        public void NewPositionButtons_pnl_Messages_Buttons() {
            btn_Messages_Incoming.Centering_Y(ToCSR(20));
            btn_Messages_Outgoing.Location = new Point(btn_Messages_Incoming.Right + ToCSR(10), btn_Messages_Incoming.Top);
            btn_Messages_Archive.Location = new Point(btn_Messages_Outgoing.Right + ToCSR(10), btn_Messages_Incoming.Top);
            btn_Messages_Write.Location = new Point(btn_Messages_Archive.Right + ToCSR(10), btn_Messages_Incoming.Top);
        }
        /// <summary> Метод вычисляет новую позицию контролов на панели pnl_Messages_Footer. </summary>
        private void NewPositionControls_pnl_Messages_Footer() {
            chb_Messages_All.Left = ToCSR(20);
            btn_Messages_Footer_Delete.Left = chb_Messages_All.Right + ToCSR(10);
            btn_Messages_Footer_InArchive.Left = btn_Messages_Footer_Delete.Right + ToCSR(10);
            btn_Messages_Footer_Read.Left = btn_Messages_Footer_InArchive.Right + ToCSR(10);
        }

        /// <summary> Логика чек бокса: "отметить всё". </summary>
        private void chb_Messages_All_CheckedChanged(object sender, EventArgs e) {
            for (int i = 0; i < grid_Messages.Rows.Count; i++) { grid_Messages[0, i].Value = chb_Messages_All.Checked; }
        }
        /// <summary> Логика безвозвратного удаления выделенных сообщений из таблицы и списка: <b>GAME.Messages.LIST[n];</b> </summary>
        private void btn_Messages_Footer_Delete_Click(object sender, EventArgs e) {
            for (int i = 0; i < grid_Messages.Rows.Count; i++) {
                if ((bool)grid_Messages[0, i].Value) { GAME.Messages.LIST.Remove((TMessage.TData)grid_Messages[0, i].Tag); }
            }
            _Update();
        }
        /// <summary> Логика добавления сообщений в архив листа <b>GAME.Messages.LIST[n];</b>. </summary>
        private void btn_Messages_Footer_InArchive_Click(object sender, EventArgs e) {
            int count = 0;
            for (int i = 0; i < grid_Messages.Rows.Count; i++) {
                if ((bool)grid_Messages[0, i].Value) {
                    if (!((TMessage.TData)grid_Messages[0, i].Tag).Archive) { count++;
                        ((TMessage.TData)grid_Messages[0, i].Tag).Archive = true; }
                    grid_Messages[0, i].Value = false;
                }
            }
            if (count > 0) { _Update();
                MessageBox.Show(LANGUAGES.Reports[29]/*В архив добавлено*/ + " " + count + " " + LANGUAGES.Messages[13]/*сообщений*/);
            }
        }
        /// <summary> Логика отметки выбранного сообщения как прочитанного. </summary>
        private void btn_Messages_Footer_Read_Click(object sender, EventArgs e) {
            for (int i = 0; i < grid_Messages.Rows.Count; i++) {
                if ((bool)grid_Messages[0, i].Value) { 
                    ((TMessage.TData)grid_Messages[0, i].Tag).Read = true; grid_Messages[0, i].Value = false;
                }
            }
            _Update();
        }

    }
}
