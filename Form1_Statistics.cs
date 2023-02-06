using System;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using static UFO.Convert;

namespace WindowsInterface
{
    public partial class Form1 : Form {
        /// <summary>
        ///     Метод сортирует листы <b>BotList</b> и <b>GAME.Alliances.LIST</b> по суммарному населению всех деревень чтобы получить значение <b>Rank.</b> <br/>
        ///     После сортировки значения полей <b>Rank</b> берутся по индексу в соответствующих листах с добавлением единицы, <br/> потому что в оригнинальной игре нет нулевого ранга. Самый первый ранг имеет значение 1.
        /// </summary>
        /// <remarks> Вариант сортировки определяется перечислением <b>TypeStatistics.</b> </remarks>
        /// <value>
        ///     <b>TypeStatistics.Players:</b> сортировка игроков. <br/>
        ///     <b>TypeStatistics.Alliances:</b> сортировка альянсов. <br/>
        /// </value>
        public void SortRank(TypeStatistics Statistics) {
            Event_Thread_Timer_Backend.Change(0, Timeout.Infinite);//ставим на паузу поток бэкенда
            if (Statistics == TypeStatistics.Players) {                 //ИГРОКИ
                if (BotList.Count > 1) BotList.Sort(delegate (TPlayer x, TPlayer y) {
                    //sort по возрастанию: слева "x", справа "y" / sort по убыванию: слева "y", справа "x"
                    return y.Total_Population.CompareTo(x.Total_Population); }
                ); for (int i = 0; i < BotList.Count; i++) BotList[i].Rank = i + 1;
            }
            else if (Statistics == TypeStatistics.Alliances) {          //АЛЬЯНСЫ
                if (GAME.Alliances.LIST.Count > 1) GAME.Alliances.LIST.Sort(delegate (TAlliance.TData x, TAlliance.TData y) {
                    //sort по возрастанию: слева "x", справа "y" / sort по убыванию: слева "y", справа "x"
                    return y.TotalPopulation.CompareTo(x.TotalPopulation); }
                ); for (int i = 0; i < GAME.Alliances.LIST.Count; i++) GAME.Alliances.LIST[i].Rank = i + 1;
            }
            Event_Thread_Timer_Backend.Change(0, GAME.SpeedGame);//снимаем с паузы поток бэкенда
        }

        /// <summary> Метод обрабатывает некорректный ввод ранга. </summary>
        private void tb_rank_Statistics_Footer_TextChanged(object sender, EventArgs e) {
            if (tb_rank_Statistics_Footer.Text == "") {  tb_rank_Statistics_Footer.Width = tb_rank_Statistics_Footer.MinimumSize.Width;
                if (tb_name_Statistics_Footer.Text == "") btn_Statistics_Footer.Enabled = false; 
                NewPositionControls_pnl_Statistics_Footer(); return; } else btn_Statistics_Footer.Enabled = true;
            bool flag = false; string New = "", t = tb_rank_Statistics_Footer.Text;
            for (int i = 0; i < t.Length; i++) if (t[i] >= 0 + 48 && t[i] <= 9 + 48) New += t[i];//копируем только цифры
            else flag = true;//обнаружен некорректный символ
            if (New != "") { tb_rank_Statistics_Footer.Text = New.TrimStart('0');
                if (tb_rank_Statistics_Footer.Text == "") tb_rank_Statistics_Footer.Text = Player.Rank.ToString();
            } else tb_rank_Statistics_Footer.Text = Player.Rank.ToString();
            //обрезаем лишние символы
            int Length = BotList.Count.ToString().Length * 8;
            if (tb_rank_Statistics_Footer.Text.Length > Length) { flag = true;
                tb_rank_Statistics_Footer.Text = tb_rank_Statistics_Footer.Text.Substring(0, Length);
            }
            //пищим и ставим каретку в конец строки
            if (flag) { SystemSounds.Beep.Play(); tb_rank_Statistics_Footer.SelectionStart = tb_rank_Statistics_Footer.Text.Length; }
            tb_name_Statistics_Footer.Text = ""; tb_name_Statistics_Footer.Width = tb_name_Statistics_Footer.MinimumSize.Width;
            tb_rank_Statistics_Footer.Size = tb_rank_Statistics_Footer.GetPreferredSize(tb_rank_Statistics_Footer.Size) + new Size(UFO.Convert.ToCSR(5), 0);
            NewPositionControls_pnl_Statistics_Footer();
        }
        /// <summary> Метод обрабатывает ввод имени аккаунта. </summary>
        private void tb_name_Statistics_Footer_TextChanged(object sender, EventArgs e) {
            if (tb_name_Statistics_Footer.Text == "") { tb_name_Statistics_Footer.Width = tb_name_Statistics_Footer.MinimumSize.Width;
                if (tb_rank_Statistics_Footer.Text == "") btn_Statistics_Footer.Enabled = false;
                NewPositionControls_pnl_Statistics_Footer(); return; } else btn_Statistics_Footer.Enabled = true;
            //обрезаем лишние символы
            int Length = GAME.Name_MaxLength;
            if (tb_name_Statistics_Footer.Text.Length > Length) { 
                tb_name_Statistics_Footer.Text = tb_name_Statistics_Footer.Text.Substring(0, Length);
                SystemSounds.Beep.Play(); tb_name_Statistics_Footer.SelectionStart = tb_name_Statistics_Footer.Text.Length; 
            }
            tb_rank_Statistics_Footer.Text = ""; tb_rank_Statistics_Footer.Width = tb_rank_Statistics_Footer.MinimumSize.Width;
            tb_name_Statistics_Footer.Size = tb_name_Statistics_Footer.GetPreferredSize(tb_name_Statistics_Footer.Size) + new Size(UFO.Convert.ToCSR(5), 0);
            NewPositionControls_pnl_Statistics_Footer();
        }
        /// <summary> Метод обрабатывает клик мыши по кнопке подсветки строки в таблице с выбранным рангом или никнеймом. </summary>
        private void btn_Statistics_Footer_Click(object sender, EventArgs e) {
            //grid_Statistics.FirstDisplayedScrollingRowIndex = 5;
            int Rank = 1, Start, Finish;
            if (tb_rank_Statistics_Footer.Text != "") { Rank = System.Convert.ToInt32(tb_rank_Statistics_Footer.Text);
                if (Rank >= BotList.Count) { Rank = BotList.Count; tb_rank_Statistics_Footer.Text = Rank.ToString(); }
            } else
            if (tb_name_Statistics_Footer.Text != "") for (int i = 0; i < BotList.Count; i++) if (BotList[i].Nick_Name == tb_name_Statistics_Footer.Text) { Rank = BotList[i].Rank; break; }
            
            Start = (Rank - 1) - (Rank - 1) % 20; if (Start < 0) Start = 0;
            Finish = Start + 19; if (Finish > BotList.Count) Finish = BotList.Count - 1;
            Update_Panels_Statistics(TypeStatistics.Auto, Start, Finish, Rank);
        }

        /// <summary> Метод обрабатывает клик по ячейкам таблицы <b>grid_Statistics.</b> </summary>
        private void grid_Statistics_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 1) if (e.RowIndex >= 0) if (grid_Statistics.Rows[e.RowIndex].Cells[1].Tag != null) {
                WinDlg_AccountProfile((TPlayer)grid_Statistics.Rows[e.RowIndex].Cells[1].Tag);//открыть окно профиля аккаунта
            }
            ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;//отмена выделения ячейки
        }
        /// <summary> Метод обрабатывает наведение на ячейку <b>grid_Statistics.</b> </summary>
        private void grid_Statistics_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2) if (e.RowIndex >= 0) {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки <b>grid_Statistics.</b> </summary>
        private void grid_Statistics_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2) if (e.RowIndex >= 0) {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.FromArgb(110, 160, 25);
            }
        }
        /// <summary> Метод вычисляет новую позицию контролов на панели pnl_Statistics_Footer. </summary>
        private void NewPositionControls_pnl_Statistics_Footer() {
            lb1_Statistics_Footer.Left = ToCSR(10);
            tb_rank_Statistics_Footer.Left = lb1_Statistics_Footer.Left + lb1_Statistics_Footer.Width + ToCSR(10);
            lb2_Statistics_Footer.Left = tb_rank_Statistics_Footer.Left + tb_rank_Statistics_Footer.Width + ToCSR(10);
            lb3_Statistics_Footer.Left = lb2_Statistics_Footer.Left + lb2_Statistics_Footer.Width + ToCSR(10);
            tb_name_Statistics_Footer.Left = lb3_Statistics_Footer.Left + lb3_Statistics_Footer.Width + ToCSR(10);
            btn_Statistics_Footer.Left = tb_name_Statistics_Footer.Left + tb_name_Statistics_Footer.Width + ToCSR(10);
            if (tb_name_Statistics_Footer.Text == "") tb_name_Statistics_Footer.Width = tb_name_Statistics_Footer.MinimumSize.Width;
            if (tb_rank_Statistics_Footer.Text == "") tb_rank_Statistics_Footer.Width = tb_rank_Statistics_Footer.MinimumSize.Width;
        }
        /// <summary> Метод вычисляет новую позицию кнопок-фильтров на панели pnl_Statistics_Buttons. </summary>
        public void NewPositionButtons_pnl_Statistics_Buttons() {
            btn_Statistics_Players.Centering_Y(ToCSR(20));
            btn_Statistics_Alliances.Location = new Point(btn_Statistics_Players.Left + btn_Statistics_Players.Width + ToCSR(10), btn_Statistics_Players.Top);
            btn_Statistics_Villages.Location = new Point(btn_Statistics_Alliances.Left + btn_Statistics_Alliances.Width + ToCSR(10), btn_Statistics_Players.Top);
            btn_Statistics_Heroes.Location = new Point(btn_Statistics_Villages.Left + btn_Statistics_Villages.Width + ToCSR(10), btn_Statistics_Players.Top);
            btn_Statistics_Wonders.Location = new Point(btn_Statistics_Heroes.Left + btn_Statistics_Heroes.Width + ToCSR(10), btn_Statistics_Players.Top);
        }

        /// <summary> Метод обновляет вкладку "Статистика". </summary>
        /// <value>
        ///   <b>Statistics:</b> enum перечисление, способ отображения информации в таблице. <br/>
        ///   <b>Start:</b> величина ранга с которого начнутся добавляться строки в таблицу. <br/>
        ///   <b>Finish:</b> величина ранга после которого добавление строк в таблицу остановится. <br/>
        ///   <b>Illumination:</b> величина ранга которая будет подсвечена цветом. <br/>
        /// </value>
        public void Update_Panels_Statistics(TypeStatistics Statistics = TypeStatistics.Players, 
                                             int Start = -1, int Finish = -1, int Illumination = -1) {
            if (Statistics == TypeStatistics.Auto) {
                if ((int)btn_Statistics_Players.Tag == 1) Statistics = TypeStatistics.Players;
                else if ((int)btn_Statistics_Alliances.Tag == 1) Statistics = TypeStatistics.Alliances;
                else if ((int)btn_Statistics_Villages.Tag == 1) Statistics = TypeStatistics.Villages;
                else if ((int)btn_Statistics_Heroes.Tag == 1) Statistics = TypeStatistics.Heroes;
                else if ((int)btn_Statistics_Wonders.Tag == 1) Statistics = TypeStatistics.Wonders;
            }
            NewPositionButtons_pnl_Statistics_Buttons();//положение кнопок на панели pnl_Statistics_Buttons

            //блок подгона габаритов Grid
            grid_Statistics.UpdateSize();
            grid_Statistics.Columns[1].AutoSizeMode = grid_Statistics.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (grid_Statistics.Bottom < pnl_Statistics_Footer.Top - ToCSR(10) ||
                grid_Statistics.Bottom > pnl_Statistics_Footer.Top - ToCSR(10))
                grid_Statistics.Height = pnl_Statistics_Footer.Top - grid_Statistics.Left - ToCSR(10);
            if (grid_Statistics.Width < pnl_Statistics_Head.Width) grid_Statistics.Width = pnl_Statistics_Head.Width;
            else pnl_Statistics_Head.Width = pnl_Statistics_Buttons.Width = pnl_Statistics_Title.Width =
                    pnl_Statistics_Footer.Width = grid_Statistics.Width;

            //лепим панели по центру
            pnl_Statistics_Head.Centering_X(ToCSR(140));
            pnl_Statistics_Buttons.Centering_X(pnl_Statistics_Head.Top + pnl_Statistics_Head.Height);
            pnl_Statistics_Title.Centering_X(pnl_Statistics_Buttons.Top + pnl_Statistics_Buttons.Height);
            grid_Statistics.Centering_X(pnl_Statistics_Title.Top + pnl_Statistics_Title.Height + ToCSR(10));
            pnl_Statistics_Footer.Centering_X(pnl_Statistics_Footer.Parent.Height - pnl_Statistics_Footer.Height - ToCSR(10));
            grid_Statistics.Height = pnl_Statistics_Footer.Top - grid_Statistics.Top - ToCSR(10);
            NewPositionControls_pnl_Statistics_Footer();//положение текста и TextBox на панели pnl_Statistics_Footer

            SortRank(Statistics);//сортировка игроков/альянсов
            int RANK = Statistics == TypeStatistics.Players ? Player.Rank : 
                Statistics == TypeStatistics.Alliances && Player.LinkOnAlliance != null ? Player.LinkOnAlliance.Rank : -1;
            int COUNT = Statistics == TypeStatistics.Players ? BotList.Count :
                Statistics == TypeStatistics.Alliances && Player.LinkOnAlliance != null ? GAME.Alliances.LIST.Count : -1;
            //if (Statistics == TypeStatistics.Players) {
                //выводим 20 строк как в оригинальной траве
                if (Start == -1 && Finish == -1) { 
                    //если переданы значения по умолчанию, то выводим страницу игрока и подсвечиваем его строку
                    Start = (RANK - 1) - (RANK - 1) % 20; if (Start < 0) Start = 0;
                    Finish = Start + 19; if (Finish > COUNT) Finish = COUNT - 1;
                    Illumination = RANK;
                }
                grid_Statistics.Rows.Clear();
                for (int i = Start; i < Finish + 1; i++) { 
                    //добавление строк
                    if (Statistics == TypeStatistics.Players) {
                    string AllianceNameAbbreviated = BotList[i].LinkOnAlliance != null ? BotList[i].LinkOnAlliance.AllianceNameAbbreviated : BotList[i].Alliance_Name;
                        grid_Statistics.Rows.Add(BotList[i].Rank, BotList[i].Nick_Name, AllianceNameAbbreviated,
                                                 BotList[i].Total_Population, BotList[i].VillageList.Count);
                        grid_Statistics[1, grid_Statistics.Rows.Count - 1].ToolTipText = "";
                        grid_Statistics.Columns[grid_Statistics.Columns.Count - 1].HeaderText = LANGUAGES.Statistics[13];/*НАСЕЛЕНИЕ*/
                    } else if (Statistics == TypeStatistics.Alliances) {
                        grid_Statistics.Rows.Add(GAME.Alliances.LIST[i].Rank,
                                     //ник нейм держателя посольства альянса
                                     GAME.Map.Cell[GAME.Alliances.LIST[i].Owner.X, GAME.Alliances.LIST[i].Owner.Y].LinkAccount.Nick_Name,
                                     GAME.Alliances.LIST[i].AllianceNameAbbreviated,
                                     GAME.Alliances.LIST[i].TotalPopulation, GAME.Alliances.LIST[i].ListAlly.Count);
                        grid_Statistics[1, grid_Statistics.Rows.Count - 1].ToolTipText = LANGUAGES.Statistics[17];/*Основатель альянса*/
                        grid_Statistics.Columns[grid_Statistics.Columns.Count - 1].HeaderText = LANGUAGES.Statistics[18];/*УЧАСТНИКИ*/
                }

                    //подсветка строки
                    if (BotList[i].Rank == Illumination) 
                        grid_Statistics.Rows[grid_Statistics.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(230, 240, 200);
                    //прикрепляем ссылку в ячейку с ником аккаунта 
                    grid_Statistics.Rows[grid_Statistics.Rows.Count - 1].Cells[1].Tag = BotList[i];
                }
            //}
        }
    }
}
