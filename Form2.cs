using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using static GameLogica.TGame.TPlayer;

namespace WindowsInterface
{
    public partial class Form2_CreateAccaunt : Form
    {
        public Form1 form1;
        public Folk Folk_Name = Folk.NULL;
        private RANDOM Random = new RANDOM(RANDOM.INIT.RandomNext);

        public Form2_CreateAccaunt(Form1 owner)
        {
            form1 = owner;// = Owner as Form1;// null;//ссыдка на форму 1
            InitializeComponent();

            ss_NationName.Items[0].Text = "";
            Color FoneAlpha = Color.FromArgb(150, 0, 0, 0);
            Color clMainBorder, ClMainText; clMainBorder = ClMainText = Color.FromArgb(0, 0, 0);//btn_Label
            Color clBorder, ClText; clBorder = Color.FromArgb(0, 0, 0); ClText = Color.FromArgb(0, 0, 0);//всё остальное
            label1.ForeColor = label2.ForeColor = label3.ForeColor = ClText;
            label4.ForeColor = label5.ForeColor = label6.ForeColor = label7.ForeColor = Color.Black;
            btn_Label.Button_Styles(FoneAlpha, FoneAlpha, FoneAlpha, clMainBorder, ClMainText, 5);
            btn_ClearTextBox.Button_Styles(Color.Transparent, Color.Transparent, Color.Red, clBorder, ClText, 2);
            btn_Start.Button_Styles(Color.Transparent, Color.Transparent, Color.Red, clBorder, ClText, 2);
            btn_Cancel.Button_Styles(Color.Transparent, Color.Transparent, Color.Red, clBorder, ClText, 2);

            //обработчик закрытия формы - Form.KeyPreview должен быть = true
            KeyUp += (s, e) => { if (e.KeyCode == Keys.Escape) ((Form)s).Close(); };           
        }
        private void btn_Cancel_Click(object sender, EventArgs e) { Close(); }
        private void btn_ClearTextBox_Click(object sender, EventArgs e) { InputName.Text = ""; }

        private void btn_Start_Click(object sender, EventArgs e) {
            //создаём игровые файлы и мир травиана
            Cursor.Current = Cursors.WaitCursor;
            if (InputName.Text == "") { MessageBox.Show(form1.LANGUAGES.RESOURSES[26]);/*Не введён игровой ник.*/ return; }
            btn_Start.Enabled = false;
            bool exists = File.Exists("DATA_BASE/ACCOUNTS/Event_Stack.DAT");

            //каждое новое создание аккаунта автоматом затирает предыдущее, нет смысла в этом блоке. но пусть пока ещё повисит
            //if (path != null) { //если в папке создан хотя бы один аккаунт.
            //    for (int i = 0; i < path.Length; i++) if (path[i].Contains(InputName.Text)) {
            //        MessageBox.Show("Название аккаунта '" + InputName.Text + "' уже существует в папке аккаунтов.\n" +
            //            "Задайте другое навание для вашего аккаунта.");
            //        return;
            //    }
            //}

            if (exists) { MessageBox.Show(
                    form1.LANGUAGES.RESOURSES[27]/*Внимание!*/ + "\n" +
                    form1.LANGUAGES.RESOURSES[34]/*Создание нового аккаунта удалит существующий, ботов и карту.*/ + "\n" +
                    form1.LANGUAGES.RESOURSES[35] + "\n" +/*Если вы не закончили этот раунд, рекомендую скопировать папку 'DATA_BASE/ACCOUNTS' сейчас*/
                    form1.LANGUAGES.RESOURSES[36] + "\n" +/*и сохранить её рядом с добавлением цифры, например: 'DATA_BASE/ACCOUNTS_1',*/
                    form1.LANGUAGES.RESOURSES[37]/*это позволит вернуться к этой партии позже и не потерять накопленный прогресс развития.*/
                );
                form1.GAME.ClearSaves(form1.GAME.PathFolderSave);
            }
            Visible = false; 
            if (!form1.Visible) form1.Show();

            form1.GAME.Map = new TMap((int)nUD_Width.Value, (int)nUD_Height.Value);
            form1.GAME.Event_Stack.Stack.Clear();//очищаем от предыдущей игры
            form1.GAME.Reports.LIST.Clear();//очищаем от предыдущей игры
            // 1/3. создание игрока
            //========================================================================
            form1.LoadProcess.Restart("1/3. " + form1.LANGUAGES.RESOURSES[28], 100);//Создание аккаунта для игрока...
                //Load settings account from file .json
                string path_json = "DATA_BASE/JSON/Default_Account.json";
                form1.Player = Newtonsoft.Json.JsonConvert.DeserializeObject<TPlayer>(File.ReadAllText(path_json));
                form1.Player.CreateAccount(InputName.Text, InputName.Text, Folk_Name);
                form1.Player.CreateHero(Folk_Name);
            form1.LoadProcess._Update();
            form1.LoadProcess.Start("1/3. " + form1.LANGUAGES.RESOURSES[29], 100);//Создание деревень для игрока...
            form1.Player.VillageList.Clear();
                //Load settings village default from file .json
                path_json = "DATA_BASE/JSON/Default_Village.json";
                var Village = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage>(File.ReadAllText(path_json));
                path_json = "DATA_BASE/JSON/Default_Village_slots" + Village.Type_Resources + ".json";
                Village.Slot = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage.TSlot[]>(File.ReadAllText(path_json));
            form1.Player.CreateVillage(Village, form1.GAME, form1.LANGUAGES);

                form1.LoadProcess._Update();
            //========================================================================
            
            bool b = false;//флаг ставили ли деревню в коры (0, 0) ТЕСТ
            // 2/3. создание ботов
            //========================================================================
            string str = "2/3. " + form1.LANGUAGES.RESOURSES[30];//Создание аккаунтов для ботов...
            form1.LoadProcess.Start(str, (int)nUD_CountOfBots.Value);
                form1.BotList.Clear(); form1.BotList.Add(form1.Player);//BotList[0] = Player
                //генератор ботов (в цикле указано количество)
                for (int i = 0; i < (int)nUD_CountOfBots.Value; i++) {
                    Folk FolkName = (Folk)Random.RND((int)Folk.Rome, (int)Folk.Gaul);
                    path_json = "DATA_BASE/JSON/Default_Account.json";
                    var bot = Newtonsoft.Json.JsonConvert.DeserializeObject<TPlayer>(File.ReadAllText(path_json));
                    bot.CreateAccount($"[_TBot {i}_]", "Bot " + i, FolkName);
                    bot.CreateHero(FolkName);
                    bot.VillageList.Clear();
                    path_json = "DATA_BASE/JSON/Default_Village.json";
                    Village = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage>(File.ReadAllText(path_json));
                    path_json = "DATA_BASE/JSON/Default_Village_slots" + Village.Type_Resources + ".json";
                    Village.Slot = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage.TSlot[]>(File.ReadAllText(path_json));
                    bot.CreateVillage(Village, form1.GAME, form1.LANGUAGES);
                    form1.BotList.Add(bot);
                    form1.LoadProcess.LoadText.Text = str + $" [{i + 1}]"; form1.LoadProcess._Update();

                    if (bot.VillageList[0].Coordinates_World_Travian == new Point(0, 0)) b = true;//ТЕСТ деревня в этих корах есть
                }
            //========================================================================
            
            //ТЕСТ, СОЗДАНИЕ БОТА В КОРАХ (0, 0) ДЛЯ ТЕСТОВ ОТПРАВКИ ВОЙСК, чтобы не было Vilalge = null на карте
            if (!b) {
                path_json = "DATA_BASE/JSON/Default_Account.json";
                var bot = Newtonsoft.Json.JsonConvert.DeserializeObject<TPlayer>(File.ReadAllText(path_json));
                bot.CreateAccount("[_TestBot Multihunter_]", "Multihunter", Folk.German);
                bot.CreateHero(Folk.German);
                bot.VillageList.Clear();
                path_json = "DATA_BASE/JSON/Default_Village.json";
                Village = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage>(File.ReadAllText(path_json));
                path_json = "DATA_BASE/JSON/Default_Village_slots" + Village.Type_Resources + ".json";
                Village.Slot = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage.TSlot[]>(File.ReadAllText(path_json));
                bot.CreateVillage(Village, form1.GAME, form1.LANGUAGES, 0, 0);
                bot.VillageList[0].Village_Name = "Mult";
                form1.BotList.Add(bot);
            }
            
            // 3/3. создание карты
            //========================================================================
            form1.LoadProcess.Start("3/3. " + form1.LANGUAGES.RESOURSES[31],/*Генерация карты*/form1.GAME.Map.Length_X() * form1.GAME.Map.Length_Y());
            form1.GAME.Map.CreateMap(form1.LoadProcess);
            form1.LoadProcess.Stop(TLoadProcess.Report.Hide);
            //========================================================================

            form1.TSM_0_Item_0.DropDownItems.Clear();//стираем меню доступных аккаунтов.
            form1.statusStrip.Items[0].Text = form1.LANGUAGES.RESOURSES[20] + " " +//Народ:
                                              form1.LANGUAGES.RESOURSES[21 + (int)form1.Player.Folk_Name];//Имя народа
            form1.statusStrip.Items[1].Text = form1.LANGUAGES.RESOURSES[24] + " " + form1.Player.Nick_Name;//Игровой ник: Имя аккаунта
            
            //ДЛЯ ТЕСТА. УДАЛИТЬ ПОТОМ ЭТИ СТРОКИ
            form1.Player.VillageList[form1.Player.ActiveIndex].HourlyProductionResources = new Res(1800, 1800, 1800, 100, 900);
            form1.Player.VillageList[form1.Player.ActiveIndex].ResourcesInStorages = new Res(50, 100, 150, 0, 35);
            //временные добавления событий в стек для теста !!!!!!!!!!!!!!!!!!!!!
            Point finish = form1.GAME.Map.Coord_WorldToMap(form1.Player.VillageList[form1.Player.ActiveIndex].Coordinates_World_Travian);//коры игрока
            Point start = form1.GAME.Map.Coord_WorldToMap(new Point(0, 0));//коры мультихантора
            //ВОЙСКА
            //RED_attack
            form1.GAME.Event_Stack.Add(60, Type_Event.ATTACK, Type_Movement.None, Folk.German, start, finish, new int[] { 100, 0, 500, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, - 1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(45, Type_Event.ATTACK, Type_Movement.None, Folk.Rome, start, finish, new int[] { 1000, 0, 5000, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(35, Type_Event.ATTACK, Type_Movement.None, Folk.Gaul, start, finish, new int[] { 0, 0, 500, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(30, Type_Event.RAID, Type_Movement.None, Folk.Nature, start, finish, new int[] { 0, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(20, Type_Event.RAID, Type_Movement.None, Folk.German, start, finish, new int[] { 0, 0, 0, 300, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            //GREEN_shield
                /*ТЕСТ*/form1.Player.Hero.isCreated = true; form1.Player.Hero.isHome = false; form1.Player.Hero.HP = 100;
                /*ТЕСТ*/form1.Player.Hero.Registration_Village = form1.Player.VillageList[0].Village_Name;
            form1.GAME.Event_Stack.Add(10, Type_Event.REINFORCEMENTS, Type_Movement.Возвращение_войск, Folk.Rome, start, finish, new int[] { 250, 625, 13000, 0, 0, 0, 0, 100, 0, 0, 1}, new int[] { 200, 200, 200, 350, 50 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(15, Type_Event.REINFORCEMENTS, Type_Movement.Возвращение_войск, Folk.Rome, start, finish, new int[] { 0, 0, 500, 0, 9640, 0, 0, 0, 0, 10, 0 }, new int[] { 300, 300, 300, 350, 50 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(30, Type_Event.REINFORCEMENTS, Type_Movement.Подкрепление, Folk.German, start, finish, new int[] { 1500, 500, 0, 0, 15, 0, 25, 0, 5, 0, 1 }, new int[] { 150, 150, 150, 150, 25 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(25, Type_Event.REINFORCEMENTS, Type_Movement.Подкрепление, Folk.Gaul, start, finish, new int[] { 220, 366, 50, 0, 0, 30, 0, 100, 0, 3, 1 }, new int[] { 150, 150, 150, 150, 25 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(20, Type_Event.REINFORCEMENTS, Type_Movement.Подкрепление, Folk.Nature, start, finish, new int[] { 0, 0, 0, 0, 0, 15, 0, 25, 10, 30, 0 }, new int[] { 75, 75, 75, 75, 15 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(5, Type_Event.REINFORCEMENTS, Type_Movement.Подкрепление, Folk.Natar, start, finish, new int[] { 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 0 }, new int[] { 50, 50, 50, 50, 10 }, -1, -1, Buildings.ПУСТО, false);
            //PURPLE_attack
            form1.GAME.Event_Stack.Add(40, Type_Event.ADVENTURE_ATTACK, Type_Movement.None, Folk.German, finish, start, new int[] { 0, 0, 0, 0, 0, 0, 450, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            form1.GAME.Event_Stack.Add(20, Type_Event.ADVENTURE_RAID, Type_Movement.None, Folk.Gaul, finish, start, new int[] { 0, 0, 0, 0, 0, 555, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            //send_settlers
            form1.GAME.Event_Stack.Add(37 * 58, Type_Event.ESTABLISH_A_SETTLEMENT, Type_Movement.None, Folk.German, finish, start, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0 }, new int[] { 750, 750, 750, 750, 0 }, -1, -1, Buildings.ПУСТО, false);
            //YELLOW_shield
            form1.GAME.Event_Stack.Add(247 * 60, Type_Event.REINFORCEMENTS, Type_Movement.Подкрепление, Folk.Rome, finish, start, new int[] { 0, 0, 0, 0, 0, 499, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            //YELLOW_attack
            form1.GAME.Event_Stack.Add(324 * 52, Type_Event.RAID, Type_Movement.None, Folk.German, finish, start, new int[] { 0, 0, 0, 0, 499, 500, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, Buildings.ПУСТО, false);
            //ПОСТРОЙКИ
            form1.GAME.Event_Stack.Add(10, Type_Event.Construction, Type_Movement.None, Folk.German, finish, finish, null, null, 7, 1, Buildings.Ферма, false);
                form1.Player.VillageList[form1.Player.ActiveIndex].Slot[7].ProcessOfConstruction = true;
                form1.Player.VillageList[form1.Player.ActiveIndex].Slot[7].ID_ProcessOfConstruction = Buildings.Ферма;
            form1.GAME.Event_Stack.Add(20, Type_Event.Construction, Type_Movement.None, Folk.German, finish, finish, null, null, 6, 1, Buildings.Ферма, false);
                form1.Player.VillageList[form1.Player.ActiveIndex].Slot[6].ProcessOfConstruction = true;
                form1.Player.VillageList[form1.Player.ActiveIndex].Slot[6].ID_ProcessOfConstruction = Buildings.Ферма;

            //ОТЧЁТЫ
            //move
            //создание деревни для отчёта с расселением
            path_json = "DATA_BASE/JSON/Default_Village.json";
            Village = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage>(File.ReadAllText(path_json));
            path_json = "DATA_BASE/JSON/Default_Village_slots" + Village.Type_Resources + ".json";
            Village.Slot = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage.TSlot[]>(File.ReadAllText(path_json));
            form1.Player.CreateVillage(Village, form1.GAME, form1.LANGUAGES);
            var NewVillagePoint = form1.Player.VillageList[1].Coordinates_Cell;

            form1.GAME.Reports.Add(Type_Report.SettlersCreateNewVillage, Type_Event.ESTABLISH_A_SETTLEMENT, false, false, finish, NewVillagePoint, "01.01.2023", "00:00:00", new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0 }, null, null, null, null, null, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0 }, new int[] { 750, 750, 750, 750, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);

            form1.GAME.Reports.Add(Type_Report.Reinforcement_Arrived, Type_Event.None, false, false, start, finish, "17.11.2022", "16:27:12", new int[] { 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, null, null, null, null, null, null, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Reinforcement_Arrived, Type_Event.None, false, false, finish, start, "17.11.2022", "13:10:00", new int[] { 0, 200, 0, 0, 300, 0, 0, 0, 0, 0, 0 }, null, null, null, null, null, null, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Reinforcement_Arrived, Type_Event.None, true, false, start, finish, "17.11.2022", "19:33:45", new int[] { 100, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, null, null, null, null, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Reinforcement_Arrived, Type_Event.None, false, true, finish, start, "17.11.2022", "09:49:01", new int[] { 0, 30, 0, 0, 0, 0, 220, 0, 0, 0, 0 }, null, null, null, null, null, null, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);

            form1.GAME.Reports.Add(Type_Report.Reinforcements_Attacked, Type_Event.None, false, false, finish, start, "20.12.2022", "10:40:00", new int[] { 0, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 150, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, null, null, new int[] { 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Reinforcements_Attacked, Type_Event.None, false, false, finish, start, "20.12.2022", "12:45:05", new int[] { 0, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 250, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, null, null, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Reinforcements_Attacked, Type_Event.None, false, false, finish, start, "20.12.2022", "13:44:08", new int[] { 0, 85, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, null, null, new int[] { 0, 85, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);

            /*Deffense_Troops*/int[] arr1 = new int[55]; arr1[0] = 50000; arr1[1] = 100000; arr1[2] = 25000; arr1[3] = 75000; arr1[4] = 3000;
            /*Deffense_Losses*/int[] arr2 = new int[55]; arr2[0] = 41224; arr2[1] = 76520; arr2[2] = 19105; arr2[3] = 62325; arr2[4] = 1600;
            form1.GAME.Reports.Add(Type_Report.Attack_Dead_RED, Type_Event.ATTACK, false, false, finish, start, "31.12.2022", "23:55:00", new int[] { 63400, 128500, 0, 0, 0, 44200, 480, 120, 0, 0, 1 }, new int[] { 63400, 128500, 0, 0, 0, 44200, 480, 120, 0, 0, 1 }, new int[] { 15000, 6500, 0, 1000, 0, 300, 0, 0, 0, 0, 1 }, new int[] { 4820, 480, 0, 825, 0, 298, 0, 0, 0, 0, 0 }, arr1, arr2, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, 12, 8, Buildings.Рынок, false, 20, 14, Buildings.Склад, true, 0, Traps.My_Troops);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[11] = 1;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[11] = 1;
            form1.GAME.Reports.Add(Type_Report.Attack_Win_GREEN, Type_Event.RAID, false, false, finish, start, "01.01.2023", "01:20:05", new int[] { 15, 0, 0, 0, 0, 0, 40, 2023, 0, 0, 1 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, arr1, arr2, new int[] { 15, 0, 0, 0, 0, 0, 40, 2023, 0, 0, 1 }, new int[] { 50, 75, 50, 30, 12 }, 10, 0, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[3] = 300; arr1[14] = 350; arr1[23] = 820; arr1[50] = 10;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[3] = 45; arr2[14] = 80; arr2[23] = 27; arr2[54] = 0;
            form1.GAME.Reports.Add(Type_Report.Attack_Losses_YELLOW, Type_Event.RAID, false, false, finish, start, "15.11.2023", "03:50:10", new int[] { 520, 360, 0, 0, 0, 0, 236, 0, 0, 0, 0 }, new int[] { 420, 160, 0, 0, 0, 0, 125, 0, 0, 0, 0 }, new int[] { 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 195, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, arr1, arr2, new int[] { 100, 200, 0, 0, 0, 0, 111, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, 18, 18, -1, -1, Buildings.ПУСТО, false, - 1, -1, Buildings.ПУСТО, false, - 1, Traps.None);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[0] = 10000; arr1[29] = 15000;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[0] = 0; arr2[29] = 0;
            form1.GAME.Reports.Add(Type_Report.Defend_Win_GREEN, Type_Event.ATTACK, false, false, start, finish, "13.11.2023", "05:42:12", new int[] { 24, 13, 0, 0, 0, 0, 5, 3, 0, 0, 1 }, new int[] { 24, 13, 0, 0, 0, 0, 5, 3, 0, 0, 1 }, new int[] { 0, 16000, 0, 9300, 800, 0, 50, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, arr1, arr2, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, 8, 6, 5, 3, Buildings.Пункт_сбора, true, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[27] = 200; arr1[50] = 100;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[27] = 100; arr2[50] = 50;
            form1.GAME.Reports.Add(Type_Report.Defend_Losses_YELLOW, Type_Event.RAID, false, true, start, finish, "01.01.2023", "07:11:26", new int[] { 300, 100, 0, 0, 0, 0, 64, 0, 0, 0, 0 }, new int[] { 200, 70, 0, 0, 0, 0, 44, 0, 0, 0, 0 }, new int[] { 0, 2000, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 75, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, arr1, arr2, new int[] { 100, 30, 0, 0, 0, 0, 20, 0, 0, 0, 0 }, new int[] { 666, 666, 666, 666, 66 }, 14, 3, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, 400, Traps.Ally_Troops);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[4] = 150; arr1[18] = 2200; arr1[25] = 840; arr1[36] = 5; arr1[45] = 500; arr1[52] = 17;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[4] = 150; arr2[18] = 2200; arr2[25] = 840; arr2[36] = 5; arr2[45] = 500; arr2[52] = 17;
            form1.GAME.Reports.Add(Type_Report.Defend_Dead_RED, Type_Event.ATTACK, false, true, start, finish, "01.01.2023", "08:15:20", new int[] { 0, 0, 732, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 702, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 0 }, arr1, arr2, new int[] { 0, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 50, 20, 75, 100, 10 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, 128, Traps.My_And_Ally_Troops);
            /*Deffense_Troops*/arr1 = new int[55]; 
            /*Deffense_Losses*/arr2 = new int[55]; 
            form1.GAME.Reports.Add(Type_Report.Defend_Win_GRAY, Type_Event.RAID, false, true, start, finish, "02.01.2023", "08:00:10", new int[] { 1000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, arr1, arr2, new int[] { 1000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 800, 800, 800, 800, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            //other                                                                                                                           //РИМ: 1:0     2:750    3:9000    4:0     5:5600     6:600
            form1.GAME.Reports.Add(Type_Report.Won_Scout_Attacker_GREEN, Type_Event.RAID, false, true, finish, start, "17.11.2022", "04:00:00", new int[] { 0, 0, 0, 125, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, null, null, new int[] { 0, 0, 0, 125, 0, 0, 0, 0, 0, 0, 0 }, null , -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            /*Deffense_Troops*/arr1 = new int[55]; arr1[9] = 5000; arr1[27] = 13625; arr1[38] = 666;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[9] = 0; arr2[27] = 0; arr2[38] = 0;
            form1.GAME.Reports.Add(Type_Report.Won_Scout_Attacker_YELLOW, Type_Event.RAID, false, false, finish, start, "17.11.2022", "22:00:00", new int[] { 0, 0, 0, 2525, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 2000, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 7500, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, arr1, arr2, new int[] { 0, 0, 0, 525, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Lost_Scout_Attacker_RED, Type_Event.RAID, true, false, finish, start, "18.11.2022", "22:00:00", new int[] { 0, 0, 0, 666, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 666, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 55000, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Won_Scout_Defender_GREEN, Type_Event.RAID, false, true, start, finish, "18.11.2022", "10:00:00", new int[] { 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, new int[] { 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Lost_Scout_Defender_YELLOW, Type_Event.RAID, false, false, start, finish, "12.11.2022", "06:30:00", new int[] { 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, null, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            //шаманим случайные коры оазиса
            Point PointOases; while (true) {
                PointOases = new Point(Random.RND(0, form1.GAME.Map.Length_X() - 1), Random.RND(0, form1.GAME.Map.Length_Y() - 1));
                if (form1.GAME.Map.Cell[PointOases.X, PointOases.Y].ID >= Cell_ID.Oasis_wood25 &&
                    form1.GAME.Map.Cell[PointOases.X, PointOases.Y].ID <= Cell_ID.Oasis_crop50) break;
            }
            /*Deffense_Troops*/arr1 = new int[55]; arr1[0] = 25; arr1[6] = 19; arr1[27] = 4; arr1[40] = 17;
            /*Deffense_Losses*/arr2 = new int[55]; arr2[0] = 14; arr2[6] = 11; arr2[27] = 2; arr2[40] = 9;
            form1.GAME.Reports.Add(Type_Report.Adventure, Type_Event.ADVENTURE_RAID, false, false, finish, PointOases, "01.01.2023", "01:20:05", new int[] { 500, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 1 }, new int[] { 78, 0, 169, 0, 0, 0, 0, 0, 0, 0, 0 }, null, null, arr1, arr2, new int[] { 422, 0, 831, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            //trading
            form1.GAME.Reports.Add(Type_Report.Mostly_wood, Type_Event.None, false, false, start, finish, "11.11.2022", "12:00:40", null, null, null, null, null, null, null, new int[] { 500, 100, 50, 25, 5 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Mostly_clay, Type_Event.None, false, true, finish, start, "23.11.2022", "15:18:20", null, null, null, null, null, null, null, new int[] { 50, 300, 30, 120, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Mostly_iron, Type_Event.None, true, false, start, finish, "01.12.2022", "23:59:59", null, null, null, null, null, null, null, new int[] { 25, 300, 600, 75, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Mostly_crop, Type_Event.None, false, false, finish, start, "05.10.2022", "01:05:20", null, null, null, null, null, null, null, new int[] { 0, 0, 0, 100, 0 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);
            form1.GAME.Reports.Add(Type_Report.Mostly_gold, Type_Event.None, false, true, start, finish, "18.11.2022", "06:28:13", null, null, null, null, null, null, null, new int[] { 0, 20, 30, 40, 45 }, -1, -1, -1, -1, Buildings.ПУСТО, false, -1, -1, Buildings.ПУСТО, false, -1, Traps.None);

            //ТЕСТ сообщений
            //должно быть по фильтрам (всего 10 шт):
            //кнопка "ВХОДЯЩИЕ" =  4 не прочитанных + 2 прочитанных = 6 шт
            //кнопка "ОТПРАВЛЕННЫЕ" = 2 шт
            //кнопка "АРХИВ" = 2 шт
            Point bot1_cell = form1.BotList[1].VillageList[0].Coordinates_Cell;
            Point bot2_cell = form1.BotList[2].VillageList[0].Coordinates_Cell;
            form1.GAME.Messages.Add(Type_Message.Incoming, start, finish, -1, false, false, "21.01.2023", "03:19:45", "Всех забаню! / I'll ban everyone!", "Hello, i am Multihunter.\r\nПривет от Мультихантера.\r\nЧё как?\r\n_________________________\r\nПишет: (Is writing:) Multihunter");
            form1.GAME.Messages.Add(Type_Message.Incoming, start, finish, 0, false, false, "22.01.2023", "01:00:00", "Дай кропа срочно! / Give me a crop urgently!", "Hello, the army is dying.\r\nIs there grain?\r\nAnd if I find it?\r\nПривет, дохнет армия.\r\nЕсть зерно?\r\nА если найду?\r\n_________________________\r\nПишет: (Is writing:) Multihunter");
            form1.GAME.Messages.Add(Type_Message.Outgoing, finish, start, -1, false, true, "23.01.2023", "14:05:01", "Муха, дай порулить / Hunter, let me steer", "1234567890\r\n_________________________\r\nПишет: (Is writing:) Player");
            form1.GAME.Messages.Add(Type_Message.Outgoing, finish, start, -1, false, false, "25.01.2023", "10:36:18", "test topic", "test message\r\n_________________________\r\nПишет: (Is writing:) Player");
            form1.GAME.Messages.Add(Type_Message.Incoming, start, finish, -1, false, true, "27.01.2023", "05:05:55", "test topic1", "test text.\r\ntest text test text test text\r\ntest text test text\r\ntest text\r\n0123\r\n_________________________\r\nПишет: (Is writing:) Multihunter");
            form1.GAME.Messages.Add(Type_Message.Incoming, start, finish, -1, true, false, "27.01.2023", "07:00:12", "test topic2", "test text.\r\ntest text test text test text\r\ntest text test text\r\ntest text\r\n0123\r\n_________________________\r\nПишет: (Is writing:) Multihunter");
            form1.GAME.Messages.Add(Type_Message.Incoming, start, finish, 2, false, false, "27.01.2023", "12:36:07", "test topic3", "test text.\r\ntest text test text test text\r\ntest text test text\r\ntest text\r\n0123\r\n_________________________\r\nПишет: (Is writing:) Multihunter");
            form1.GAME.Messages.Add(Type_Message.Incoming, bot1_cell, finish, -1, false, true, "01.02.2023", "21:15:00", "I'm bot?", "bot bot bot bot bot\r\n_________________________\r\nПишет: (Is writing:) bot 1");
            form1.GAME.Messages.Add(Type_Message.Incoming, bot2_cell, finish, -1, false, false, "01.02.2023", "23:04:10", "Есть ли у ботов сознание? / Do bots have consciousness?", "bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot bot 123\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\nbot\r\n123\r\n_________________________\r\nПишет: (Is writing:) bot 2");

            //ТЕСТ шаманим случайное население игрокам
            for (int i = 0; i < form1.BotList.Count; i++) form1.BotList[i].Total_Population = Random.RND(2, 1000);

            //ТЕСТ создания альянса и добавления участников в него
            //проверка на кол-во символов для полного названия и сокращения
            //проверяется в окне создания/переименования альянса !!!!!
            form1.GAME.Alliances.Add("SLO", "S+Legion+O", form1.Player.VillageList[0].Coordinates_Cell);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.Player);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.BotList[1]);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.BotList[4]);
            form1.GAME.Alliances.Add("Celts", "Celts Of Prairie", form1.BotList[6].VillageList[0].Coordinates_Cell);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.BotList[6]);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.BotList[9]);
            form1.GAME.Alliances.Add("Muha", "Alliance Of Multihunter", form1.BotList[form1.BotList.Count - 1].VillageList[0].Coordinates_Cell);
            form1.GAME.Alliances.LIST[form1.GAME.Alliances.LIST.Count - 1].AddNewMember(form1.BotList[form1.BotList.Count - 1]);

            //ТЕСТ сортируем ранги
            form1.SortRank(TypeStatistics.Players); form1.SortRank(TypeStatistics.Alliances);


            form1.GAME.Map.Location = form1.Player.VillageList[form1.Player.ActiveIndex].Coordinates_World_Travian;
            form1.btn_Construction.Enabled = true;//удалить в релизе

            if (form1.tabControl.SelectedIndex != 1) form1.tabControl.SelectedIndex = 1;
            else form1.TabControl_SelectedIndexChanged();//к ресурсным полям - последняя строка!

            form1.BackGroundResources();
            form1.SetLanguage();//перевести весь текст

            Cursor.Current = Cursors.Default;
            Close();
        }

        private void Form2_CreateAccaunt_Load(object sender, EventArgs e) {
            InputName.MaxLength = form1.GAME.Name_MaxLength;//максимальная длинна строки
            InputName.Focus();
            //BackgroundImage = Image.FromFile("DATA_BASE/IMG/MENU/registration" + Random.RND(1, 2) + ".png");
            //BackColor = Color.FromArgb(20, 20, 20);
            if (ss_NationName.Items[0].Text == "") ss_NationName.Items[0].Text = form1.LANGUAGES.RESOURSES[26];//Не введён игровой ник.
        }

        /// <summary> Метод обрабатывает некорректный ввод ник нэйма. С этим именем создаётся папка аккаунта. </summary>
        private void InputName_TextChanged(object sender, EventArgs e) {
            if (InputName.Text == "") { btn_Start.Enabled = true;
                ss_NationName.Items[0].Text = form1.LANGUAGES.RESOURSES[26];
                return; 
            }
            ss_NationName.Items[0].Text = form1.LANGUAGES.RESOURSES[21 + (int)Folk_Name]/*Имя народа*/
                + " :: " + InputName.Text;
            bool flag = false; string New = "", t = InputName.Text;
            for (int i = 0; i < t.Length; i++)
                if (t[i] != '/' && t[i] != '\\' && t[i] != ':' && t[i] != '*' && t[i] != '?' && t[i] != '"' && t[i] != '<'
                    && t[i] != '>' && t[i] != '[' && t[i] != ']' && t[i] != '.'
                   ) { New += t[i]; btn_Start.Enabled = true; } else flag = true;//Обнаружен некорректный ввод символа.
            //Поставить каретку в конец строки и пропищать
            if (flag) { btn_Start.Enabled = false; SystemSounds.Beep.Play(); InputName.Text = New; InputName.SelectionStart = InputName.Text.Length; }
        }

        /// <summary> Метод обрабатывает изменение свойства <b>Value</b> у объекта <b>NumericUpDown.</b> </summary>
        private void nUD_Width_ValueChanged(object sender, EventArgs e) {
            label4.Text = "X: [-" + nUD_Width.Value + "..0; 0..+" + nUD_Width.Value + "]";
            label5.Text = "Y: [-" + nUD_Height.Value + "..0; 0..+" + nUD_Height.Value + "]";
            label6.Text = form1.LANGUAGES.RESOURSES[32]/*Массив:*/ + " " +
                          "[0.." + (nUD_Width.Value + nUD_Width.Value) + "][0.."
                          + (nUD_Height.Value + nUD_Height.Value) + "]";
            label7.Text = form1.LANGUAGES.RESOURSES[33]/*Ботов:*/ + " " + nUD_CountOfBots.Value;
        }

        /// <summary> Метод обрабатывает клик по форме. </summary>
        private void Form2_CreateAccaunt_Click(object sender, EventArgs e) { Focus(); }

        /// <summary> Двойной клик заполняет поля ввода по умолчанию. </summary>
        private void Form2_CreateAccaunt_DoubleClick(object sender, EventArgs e) {
            nUD_Width.Value = nUD_Height.Value = 10; nUD_CountOfBots.Value = 10;
        }

        /// <summary> Метод ресует пунктрирную рамку на кнопках. </summary>
        private void btn_Label_Paint(object sender, PaintEventArgs e) {
            ((Button)sender).FlatAppearance.BorderSize = 0;//рамки быть не должно
            if (((Button)sender).Name == "btn_Label")
                CustomControls.DottedLine(e, BackColor, 2, 2, 1, (Button)sender);//рисуем свою рамку
            else CustomControls.DottedLine(e, Color.Black, 1, 4, 2, (Button)sender);
        }
    }
}
