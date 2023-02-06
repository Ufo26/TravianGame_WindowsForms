using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;

namespace WindowsInterface {
    interface IForm1_SetLanguage {
        /// <summary> Метод устанавливает весь существующий текст в игре на текст выбранного языка в поле <b> Player.Language </b>. </summary>
        void SetLanguage();
    }

    public partial class Form1 : Form, IForm1_SetLanguage {

        /// <summary> Содержит текстовые массивы локализации. Каждое поле-массив хранит строки из файлов выбранного языка. </summary>
        public class TLANGUAGES {
            //[JsonProperty("buildings")]//текст в кавычках = альтернативному названию этой переменной в файле resources.json
            /// <summary> Текстовый массив хранящий все строки из файла DATA_BASE<b>/</b>LANGUAGES<b>/</b>[ЯЗЫК]<b>/</b><b>Resources.json </b>. </summary>
            ///<remarks> Хранит список языков. </remarks>
            public string[] languages;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит список построек. </remarks>
            public string[] buildings;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит все строки из меню. </remarks>
            public string[] menu;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит разные данные игры которые не вошли в остальные массивы. </remarks>
            public string[] RESOURSES;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит текстовые метки панели загрузки игры. </remarks>
            public string[] TLoadProcess;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит title всплывающих подсказок. </remarks>
            public string[] tool_tip_TITLE;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит text всплывающих подсказок. </remarks>
            public string[] tool_tip_TEXT;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит многострочный text всплывающей подсказки [22]. </remarks>
            public string[] tool_tip_22;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит названия войск Натаров. </remarks>
            public string[] Natar_Name;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит названия войск дикой природы. </remarks>
            public string[] Animal_Name;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит названия войск Галлов. </remarks>
            public string[] Gaul_Name;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит названия войск Германцев. </remarks>
            public string[] German_Name;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит названия войск Римлян. </remarks>
            public string[] Rome_Name;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит 3 строки: ч:м:с для вывода времени. </remarks>
            public string[] Time;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит текст для вкладки "Статистика". </remarks>
            public string[] Statistics;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит текст для вкладки "Отчёты". </remarks>
            public string[] Reports;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит текст для вкладки "Сообщения". </remarks>
            public string[] Messages;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит текст для окна "о программе". </remarks>
            public string[] About;
            /// <summary> <inheritdoc cref = "languages"> </inheritdoc> </summary>
            ///<remarks> Хранит ошибки выполнения программы. </remarks>
            public string[] Errors;
        }
        /// <summary>
        ///     Экземпляр класса TLANGUAGES. <br/> Хранит текстовые массивы в которых хранятся все строки выбранного языка. <br/>
        ///     С помощью этого объекта осуществляется чтение строк выбранного языка в различные контроллы и элементы интерфейса.
        /// </summary>
        public TLANGUAGES LANGUAGES = null;

        public void SetLanguage() {
            //string TMP = Newtonsoft.Json.JsonConvert.SerializeObject(LANGUAGES);//создать json строку из объекта
            //File.WriteAllText("DATA_BASE/LANGUAGES/GERMAN.json", TMP);//сохранить json строку в файл
            //LANGUAGES = Newtonsoft.Json.JsonConvert.DeserializeObject<TLANGUAGES>(TMP);//загрузить в объект json строку из string
            //LANGUAGES = Newtonsoft.Json.JsonConvert.DeserializeObject<TLANGUAGES>(File.ReadAllText("DATA_BASE/LANGUAGES/GERMAN.json"));//загрузить в объект json строку из файла
            string path = Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/Resources.json";
            if (File.Exists(path)) { LANGUAGES = JsonConvert.DeserializeObject<TLANGUAGES>(File.ReadAllText(path)); }

            string[] tmp = Directory.GetDirectories("DATA_BASE/LANGUAGES/");
            if (tmp == null) { MessageBox.Show(LANGUAGES.Errors[7]/*Error 11.*/ + " Form1.SetLanguage(...)\n" +
                LANGUAGES.Errors[8]/*Каталоги языков в: 'DATA_BASE/LANGUAGES/...' не найдены.*/); return; }
            else if (GAME.Language >= tmp.Length) { MessageBox.Show(LANGUAGES.Errors[9]/*Error 12.*/ + " Form1.SetLanguage(...)\n" +
                LANGUAGES.Errors[10]/*значение 'index'*/ + " (" + GAME.Language + ") " +
                LANGUAGES.Errors[11]/*больше фактического количества языков*/ + " (" + tmp.Length + ")."); return;
            }

            //кнопка смены фоновой картинки на вкладке "Деревня"
            btn_NextbgVillage.Text = LANGUAGES.RESOURSES[132];/*Сменить фон*/ btn_NextbgVillage.AutoSize();

            //form2
            form2.btn_Label.Text = LANGUAGES.RESOURSES[113];//РЕГИСТРАЦИЯ
            form2.label1.Text = LANGUAGES.RESOURSES[114];//ВВЕДИТЕ ИГРОВОЙ НИК (до 30 символов):
            form2.label2.Text = LANGUAGES.RESOURSES[115];//РАЗМЕР КАРТЫ:
            form2.label3.Text = LANGUAGES.RESOURSES[116];//КОЛ-ВО БОТОВ:
            form2.label6.Text = LANGUAGES.RESOURSES[32]/*Массив:*/ + " " +
              "[0.." + (form2.nUD_Width.Value + form2.nUD_Width.Value) + "][0.."
              + (form2.nUD_Height.Value + form2.nUD_Height.Value) + "]";
            form2.label7.Text = LANGUAGES.RESOURSES[33]/*Ботов:*/ + " " + form2.nUD_CountOfBots.Value;
            form2.label8.Text = LANGUAGES.RESOURSES[120];//Двойной клик заполняет поля ввода по умолчанию.
            form2.ss_NationName.Items[0].Text = LANGUAGES.RESOURSES[21 + (int)Player.Folk_Name];//Имя народа
            form2.btn_ClearTextBox.Text = LANGUAGES.RESOURSES[117];//ОЧИСТИТЬ ВВОД
            form2.btn_Start.Text = LANGUAGES.RESOURSES[118];//СОЗДАТЬ МИР TRAVIAN
            form2.btn_Cancel.Text = LANGUAGES.RESOURSES[119];//ОТМЕНА

            //Вкладки tabControl
            tabPage1.Text = LANGUAGES.RESOURSES[101];/*Главное меню*/tabPage2.Text = LANGUAGES.RESOURSES[102];//Ресурсные поля
            tabPage3.Text = LANGUAGES.RESOURSES[103];/*Деревня*/     tabPage4.Text = LANGUAGES.RESOURSES[104];//Карта
            tabPage5.Text = LANGUAGES.RESOURSES[105];/*Статистика*/  tabPage6.Text = LANGUAGES.RESOURSES[106];//Отчёты
            tabPage7.Text = LANGUAGES.RESOURSES[107];/*Сообщения*/   tabPage8.Text = LANGUAGES.RESOURSES[108];//Дерево построек
            bg_BuildingTree_T4.BackgroundImage = Image.FromFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/BuildingTree_T4.png");

            //МЕНЮ :: MenuStrip
            //Игра
            TSM0.Text = LANGUAGES.menu[1];
            TSM0.DropDownItems[0].Text = LANGUAGES.menu[2]; TSM0.DropDownItems[1].Text = LANGUAGES.menu[3];
            TSM0.DropDownItems[3].Text = LANGUAGES.menu[4]; TSM0.DropDownItems[4].Text = LANGUAGES.menu[5];
            TSM0.DropDownItems[6].Text = LANGUAGES.menu[6];
            //Язык
            if (TSM1 != null) TSM1.Text = LANGUAGES.menu[0];/*Язык*/
            if (TSM1.DropDownItems.Count > 0) {
                //"languages": [ "Русский / RU", "Немецкий / DE", "Английский / EN" ]
                for (int i = 0; i < tmp.Length; i++) { TSM1.DropDownItems[i].Text = LANGUAGES.languages[i]; }
            }
            //Справка
            TSM2.Text = LANGUAGES.menu[7]; TSM2.DropDownItems[0].Text = LANGUAGES.menu[8];


            //ВСПЛЫВАЮЩИЕ ПОДСКАЗКИ :: tool_tip
            if (tool_tip != null) {
                for (int i = 0; i < tool_tip.Length; i++) if (tool_tip[i] != null) { tool_tip[i].Dispose(); tool_tip[i] = null; }
                tool_tip = null;
            }
            if (tool_tip == null) tool_tip = new ToolTip[39];
            if (GAME.ToolTipFlag) {
                for (int i = 0; i < tool_tip.Length; i++) { int j = i;
                    Control c = null; bool b = false; int AutoPopDelay = 30000;
                    switch (i) {
                        case 0: c = pb_Button_Resourses; b = true; break;   case 1: c = pb_Button_Village; b = true; break;
                        case 2: c = pb_Button_Map; b = true; break;         case 3: c = pb_Button_Statistics; b = true; break;
                        case 4: c = pb_Button_Reports; b = true; break;     case 5: c = pb_Button_Messages; b = true; break;
                        case 6: c = btn_Rename_Village; b = false; break;   case 7: c = Panel_Resources; b = false; break;
                        case 8: c = Panel_Village1; b = false; break;       case 9: c = Panel_Village2; b = false; break;
                        case 10: c = GroupBox_Village; b = false; break;    case 11: c = Panel_Resources_Production; b = false; break;
                        case 12: c = Picture_Ethnos; b = false; break;      case 13: c = lb_Ethnos; b = false; break;
                        case 14: c = btn_Rome; b = true; break;             case 15: c = btn_Teutons; b = true; break;
                        case 16: c = btn_Gauls; b = true; break;
                        case 17: case 18: case 19: case 20: c = this; b = false; break;/*описаны в Form1_DialogWindows.winDlg_LevelUp_Builds(...)*/
                        case 21: c = pnl_Map_Size; b = false; break;        case 22: c = nud_value_size_map; b = false; break;
                        case 23: c = lb_text_size_map; b = false; break;    case 24: c = btn_Map_Full_Screen; b = false; break;
                        case 25: c = pnl_Map_Input_Coord; b = false; break; case 26: c = txt_Map_X; b = false; break;
                        case 27: c = txt_Map_Y; b = false; break;           case 28: c = pb_XXXX_Map; b = false; break;
                        case 29: c = bg_Map; b = false; break;              case 30: c = Panel_Army; b = false; break;
                        case 31: c = Panel_Move_Units; b = false; break;    case 32: c = Panel_Construction; b = false; break;
                        case 33: c = this; b = false; break;/*описан в Form1_DialogWindows.WinDlg_ReadMessage(...)*/
                        case 34: c = btn_Map_GetterTools; b = false; break; 
                        case 35: c = this; b = false; break;/*описан в Form1_Map.WinDlg_Map_GetterTools()*/
                        //эти кейсы переписывать чтобы были в конце, "j" указывает правильный элемент в массиве
                        //DataGridWiew не отображает всплывающие подсказки ToolTip, т.к. у каждой ячейки своя всплывающая подсказка
                        case 36: c = grid_Resources_Production; b = true; j = 11; break; case 37: c = grid_Army; b = true; j = 30; break;
                        case 38: c = grid_Move_Units; b = true; j = 31; break;
                    }
                    string Title = LANGUAGES.tool_tip_TITLE[j]; string Text = LANGUAGES.tool_tip_TEXT[j];
                    string[] title = Title.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] txt = Text.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                    string TITLE = "", TXT = "";
                    for (int it = 0; it < title.Length; it++) TITLE += title[it] + "\n";
                    for (int it = 0; it < txt.Length; it++) TXT += txt[it] + "\n";

                    tool_tip[j] = Extensions.CreateHint(c, 0, AutoPopDelay, TITLE, TXT, Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, b);
                }
                //tool_tip[22] использует больше 2 строк, а в файле tool_tip.txt 1 строка = 1 элементу в массиве tool_tip[N],
                //чтобы сохранить одинаковое кол-во строк в файле и в массиве, применяем отдельное присвоение к tool_tip[22]
                //из отдельного файла
                string text = LANGUAGES.tool_tip_22[1] + " " +
                    nud_value_size_map.Minimum.ToString() + "x" + nud_value_size_map.Minimum.ToString() + " " +
                    LANGUAGES.tool_tip_22[2] + " " +
                    nud_value_size_map.Maximum + "x" + nud_value_size_map.Maximum + " " + "\n" + LANGUAGES.tool_tip_22[3];
                tool_tip[22] = Extensions.CreateHint(nud_value_size_map, 0, 30000,
                    LANGUAGES.tool_tip_22[0], text,
                    Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, true);
            }

            //toolStripMenuItem
            toolStripMenuItem3.Text = LANGUAGES.RESOURSES[0];//"на единицу (+/- 1)";
            toolStripMenuItem4.Text = LANGUAGES.RESOURSES[1] + " " + (int)(nud_value_size_map.Value / 2) + ")";//на пол страницы (+/-
            toolStripMenuItem5.Text = LANGUAGES.RESOURSES[2] + " " + nud_value_size_map.Value + ")";//на страницу (+/-

            //Главное меню::Кнопки
            btn_Rome.Text = LANGUAGES.RESOURSES[21].ToUpper();//РИМЛЯНЕ
            btn_Teutons.Text = LANGUAGES.RESOURSES[22].ToUpper();//ГЕРМАНЦЫ
            btn_Gauls.Text = LANGUAGES.RESOURSES[23].ToUpper();//ГАЛЛЫ
                                                               //панель передвижений войск, поселенцев, говорунов и т.д.
            lb_Move_Units_Header.Text = LANGUAGES.RESOURSES[91];//Передвижения войск:
                                                                //панель строительства построек
            lb_Construction_Header.Text = LANGUAGES.RESOURSES[96];//Строительство
            btn_Construction.Text = LANGUAGES.RESOURSES[97];//Завершить моментально
            btn_Construction.AutoSize();

            //Вкладка "Статистика"
            //при изменении свойства Text, следует AutoSize ставить в false, потом в true, иначе авторазмер применяется не корректно 
            lb_Statistics_Head.Text = LANGUAGES.Statistics[0];/*Статистика*/
            btn_Statistics_Players.Text = LANGUAGES.Statistics[1];/*Игроки*/
            btn_Statistics_Alliances.Text = LANGUAGES.Statistics[2];/*Альянсы*/
            btn_Statistics_Villages.Text = LANGUAGES.Statistics[3];/*Деревни*/
            btn_Statistics_Heroes.Text = LANGUAGES.Statistics[4];/*Герои*/
            btn_Statistics_Wonders.Text = LANGUAGES.Statistics[5];/*Чудо света*/
            lb_Statistics_Title.Text = LANGUAGES.Statistics[6];/*Лучшие игроки*/
            lb1_Statistics_Footer.Text = LANGUAGES.Statistics[11];/*Ранг*/
            lb2_Statistics_Footer.Text = LANGUAGES.Statistics[15];/*или*/
            lb3_Statistics_Footer.Text = LANGUAGES.Statistics[16];/*Имя*/
            NewPositionButtons_pnl_Statistics_Buttons();
            //строки таблицы: column = колонка, row = строка [col, row] = [x, y]
            //dgw_Statistics[0, 0].Value = LANGUAGES.Statistics[11].ToUpper();/*РАНГ*/
            grid_Statistics.Columns[0].HeaderText = LANGUAGES.Statistics[11].ToUpper();/*РАНГ*/
            grid_Statistics.Columns[1].HeaderText = LANGUAGES.Statistics[1].ToUpper();/*ИГРОКИ*/
            grid_Statistics.Columns[2].HeaderText = LANGUAGES.Statistics[12];/*АЛЬЯНС*/
            grid_Statistics.Columns[3].HeaderText = LANGUAGES.Statistics[13];/*НАСЕЛЕНИЕ*/
            if (btn_Statistics_Players.Tag != null && (int)btn_Statistics_Players.Tag == 1)
                grid_Statistics.Columns[4].HeaderText = LANGUAGES.Statistics[14];/*ДЕРЕВНИ*/
            else if (btn_Statistics_Alliances.Tag != null && (int)btn_Statistics_Alliances.Tag == 1)
                grid_Statistics.Columns[4].HeaderText = LANGUAGES.Statistics[18];/*УЧАСТНИКИ*/
            //временно так. после добавления других фильтров, добавить ещё else
            else grid_Statistics.Columns[4].HeaderText = LANGUAGES.Statistics[14];/*ДЕРЕВНИ*/

            //Вкладка "Отчёты"
            lb_Reports_Head.Text = LANGUAGES.Reports[0];/*Отчёты*/
            btn_Reports_All.Text = LANGUAGES.Reports[1];/*Все*/
            btn_Reports_Army.Text = LANGUAGES.Reports[2];/*Войска*/
            btn_Reports_Trading.Text = LANGUAGES.Reports[3];/*Торговля*/
            btn_Reports_Other.Text = LANGUAGES.Reports[4];/*Другое*/
            btn_Reports_Archive.Text = LANGUAGES.Reports[5];/*Архив*/
            btn_Reports_Neighborhood.Text = LANGUAGES.Reports[6];/*Окрестность*/
            chb_Reports_All.Text = LANGUAGES.Reports[9];/*Отметить всё*/
            btn_Reports_Footer_Delete.Text = LANGUAGES.Reports[10];/*Удалить*/
            btn_Reports_Footer_InArchive.Text = LANGUAGES.Reports[11];/*В архив*/
            btn_Reports_Footer_Read.Text = LANGUAGES.Reports[89];/*Отметить прочитанным*/
            grid_Reports.Columns[3].HeaderText = LANGUAGES.Reports[7];/*Событие:*/
            grid_Reports.Columns[5].HeaderText = LANGUAGES.Reports[8];/*Дата:*/

            //Вкладка "Сообщения"
            lb_Messages_Head.Text = LANGUAGES.Messages[0];/*Сообщения*/
            btn_Messages_Incoming.Text = LANGUAGES.Messages[1];/*Входящие*/
            btn_Messages_Outgoing.Text = LANGUAGES.Messages[2];/*Отправленные*/
            btn_Messages_Write.Text = LANGUAGES.Messages[3];/*Написать*/
            btn_Messages_Archive.Text = LANGUAGES.Messages[4];/*Архив*/
            chb_Messages_All.Text = LANGUAGES.Messages[5];/*Отметить всё*/
            btn_Messages_Footer_Delete.Text = LANGUAGES.Messages[6];/*Удалить*/
            btn_Messages_Footer_InArchive.Text = LANGUAGES.Messages[7];/*В архив*/
            btn_Messages_Footer_Read.Text = LANGUAGES.Messages[8];/*Отметить прочитанным*/
            grid_Messages.Columns[2].HeaderText = $" {LANGUAGES.Messages[9]} ";/*Тема:*/
            if (btn_Messages_Outgoing.Tag != null)
                grid_Messages.Columns[3].HeaderText = (int)btn_Messages_Outgoing.Tag == 1 ?
                    $" {LANGUAGES.Messages[20]} "/*Получатель:*/ : $" {LANGUAGES.Messages[14]} "/*Отправитель:*/;
            else grid_Messages.Columns[3].HeaderText = $" {LANGUAGES.Messages[14]} "/*Отправитель:*/;
            grid_Messages.Columns[4].HeaderText = $" {LANGUAGES.Messages[10]} ";/*Дата:*/

            btn_Map_GetterTools.Text = LANGUAGES.RESOURSES[151];/*Альтернативная карта*/

            //Этот блок оставить последним
            if (Player.Folk_Name != Folk.NULL) { //игра создана/открыта
                //Информационная панель внизу главной формы :: statusStrip + :: lb_Ethnos
                statusStrip.Items[0].Text = LANGUAGES.RESOURSES[20] + " " +//Народ:
                    LANGUAGES.RESOURSES[21 + (int)Player.Folk_Name];//Имя народа
                statusStrip.Items[1].Text = LANGUAGES.RESOURSES[24] + " " + Player.Nick_Name;//Игровой ник: Имя аккаунта
                lb_Ethnos.Text = LANGUAGES.RESOURSES[21 + (int)Player.Folk_Name];//Имя народа

                Update_Panel_Resources_Production();//обновляет панель добычи ресурсов новыми данными
                Update_Panel_VillageInfo();//обновляет панель информации о деревне
                //панель списка деревень
                GroupBox_Village.Text = LANGUAGES.RESOURSES[46]/*Деревни*/ + " " + Player.VillageList.Count + "/" + Player.Limit_Village;
                //Обновление названий военных юнитов всех фракций (рим, германцы, галлы, природа, натары)
                for (int i = 0; i < GAME.Troops.Length; i++) for (int j = 0; j < GAME.Troops[i].Information.Length; j++) {
                        if (i == 0) GAME.Troops[i].Information[j].Name = LANGUAGES.Rome_Name[j]; else
                        if (i == 1) GAME.Troops[i].Information[j].Name = LANGUAGES.German_Name[j]; else
                        if (i == 2) GAME.Troops[i].Information[j].Name = LANGUAGES.Gaul_Name[j]; else
                        if (i == 3) GAME.Troops[i].Information[j].Name = LANGUAGES.Animal_Name[j]; else
                        if (i == 4) GAME.Troops[i].Information[j].Name = LANGUAGES.Natar_Name[j];
                    }
                Update_Panel_Army();//обновляет панель списка армии в выбранной деревне
                Update_Panel_Troop_Movements();//панель передвижений войск
                Update_Panel_Construction();//панель запуска построек
                //Update_Panels_Statistics();//панели вкладки "Статистика"
                Update_Panels_Reports();//панели вкладки "Отчёты"
                Update_Panels_Messages();//панели вкладки "Сообщения"
                //панель на вкладке карта
                txt_Map_DATA.Text = LANGUAGES.RESOURSES[7];/*Данные*/
                txt_Map_DATA.Left = (pnl_Map_DATA.Width - txt_Map_DATA.Width) / 2;//лепим по центру
                //назначаем названия построек в списке построек у класса Player
                for (int ID = 0; ID < GAME.Build.Length; ID++) GAME.Build[ID].Name = LANGUAGES.buildings[ID];
                //ComboBox. уничтожаем объект, чтобы в окне выбора уровня постройки создать его снова
                //и назначить названия построек в выпадающем списке снова согласно выбранному языку
                if (Build_List != null) { Build_List.Dispose(); Build_List = null; }
                GAME.Event_Stack.Languages = LANGUAGES;//передаём в стек это поле чтобы там можно было выводить ошибки на всех языках
            }
        }

        /// <summary> Метод конвертирует значение <b>enum Type_Report</b> в эквивалент текста на текущем языке. </summary>
        /// <value>
        ///     <b><paramref name="TypeReport"/>:</b> тип отчёта. <br/>
        ///     <b><paramref name="bgCell_1"/>:</b> цвет первой части отчёта: (нападение, разведка, приключение, поимка животных, торговля, расселение, подкрепление и т.д.) <br/>
        ///     <b><paramref name="bgCell_2"/>:</b> цвет второй части отчёта: (оборона, контрразведка) <br/>
        /// </value>
        /// <returns> Возвращает массив из 2 элементов. Эквивалент текста на текущем языке, соответствующий перечислению <b>enum Type_Report</b> <br/> [0] = Нападение; [1] = Оборона. </returns>
        public string[] TypeReport_To_LanguagesReports(Type_Report TypeReport, out Color bgCell_1, out Color bgCell_2) {
            Color Green = Color.FromArgb(70, 140, 70); bgCell_1 = Color.White; bgCell_2 = Color.White;
            //второй элемент массива только для таблиц обороны и контрразведки
            string[] StringReport = new string[] {$"Error. TypeEvent={TypeReport}", $"Error. TypeEvent={TypeReport}"};
            var _ = TypeReport;
            if (_ == Type_Report.Attack_Win_GREEN || _ == Type_Report.Attack_Losses_YELLOW ||
                _ == Type_Report.Attack_Dead_RED || _ == Type_Report.Defend_Win_GREEN ||
                _ == Type_Report.Defend_Losses_YELLOW || _ == Type_Report.Defend_Dead_RED ||
                _ == Type_Report.Defend_Win_GRAY || _ == Type_Report.Miscellaneous) {
                StringReport[0] = $" {LANGUAGES.Reports[48]} ";/*Нападение*/ bgCell_1 = Color.FromArgb(210, 0, 0);
                StringReport[1] = $" {LANGUAGES.Reports[49]} ";/*Оборона*/   bgCell_2 = Green;
            } else if (_ == Type_Report.Reinforcement_Arrived) {
                StringReport[0] = $" {LANGUAGES.Reports[50]} ";/*Подкрепление*/ bgCell_1 = Green;
                StringReport[1] = $" {LANGUAGES.Reports[79]} ";/*Получатель*/ bgCell_2 = Green;
            } else if (_ == Type_Report.Adventure) {
                StringReport[0] = $" {LANGUAGES.Reports[51]} ";/*Приключение*/ bgCell_1 = Color.FromArgb(0, 0, 128);
                StringReport[1] = $" {LANGUAGES.Reports[49]} ";/*Оборона*/     bgCell_2 = Green;
            } else if (_ == Type_Report.Animals_Caught) {
                StringReport[0] = $" {LANGUAGES.Reports[52]} ";/*Поимка животных*/ bgCell_1 = Color.FromArgb(150, 0, 150);
            } else if (_ == Type_Report.Won_Scout_Attacker_GREEN || _ == Type_Report.Won_Scout_Attacker_YELLOW ||
                _ == Type_Report.Lost_Scout_Attacker_RED || _ == Type_Report.Won_Scout_Defender_GREEN ||
                _ == Type_Report.Lost_Scout_Defender_YELLOW) { 
                StringReport[0] = $" {LANGUAGES.Reports[53]} ";/*Разведка*/ bgCell_1 = Color.FromArgb(200, 150, 0);
                StringReport[1] = $" {LANGUAGES.Reports[49]} ";/*Оборона*/  bgCell_2 = Green;
            } else if (_ == Type_Report.Mostly_wood || _ == Type_Report.Mostly_clay || _ == Type_Report.Mostly_iron ||
                _ == Type_Report.Mostly_crop || _ == Type_Report.Mostly_gold) {
                StringReport[0] = $" {LANGUAGES.Reports[55]} ";/*Торговля*/ bgCell_1 = Color.FromArgb(128, 128, 128);
                StringReport[1] = $" {LANGUAGES.Reports[79]} ";/*Получатель*/ bgCell_2 = Color.FromArgb(128, 128, 128);
            } else if (_ == Type_Report.Reinforcements_Attacked) {
                StringReport[0] = $" {LANGUAGES.Reports[56]} ";/*Подкрепление*/ bgCell_1 = Green;
                StringReport[1] = $" {LANGUAGES.Reports[49]} ";/*Оборона*/      bgCell_2 = Green;
            } else if (_ == Type_Report.SettlersCreateNewVillage) { 
                StringReport[0] = $" {LANGUAGES.Reports[57]} ";/*Расселение*/ bgCell_1 = Color.Black; 
                StringReport[1] = $" {LANGUAGES.Reports[81]} ";/*Новое поселение*/ bgCell_2 = Color.Black; }
            return StringReport;
        }
    }
}
