
using Global_Var;
using System;
using System.Drawing;
using System.Windows.Forms;

/// <summary> static class позволяющий перемещать объекты на форме. Drag And Drop. </summary>
public static class ControlMover {
        private static bool ChangeCursor{ get; set; }
        private static bool AllowMove { get; set; }
        private static bool AllowResize { get; set; }
        private static bool BringToFront { get; set; }
        private static int ResizingMargin { get; set; }
        private static int MinSize { get; set; }
 
        private static Point startMouse;
        private static Point startLocation;
        private static Size startSize;
        private static bool resizing = false;
        private static Cursor oldCursor;
 
        static ControlMover() {
            ResizingMargin = 5;     MinSize = 10;
            ChangeCursor = false;   AllowMove = true;
            AllowResize = true;     BringToFront = true;
        }

        /// <summary>
        ///     Метод подцепляет контрол <b> ctrl </b> к своим обработчикам событий (ctrl_MouseDown, ctrl_MouseUp, ctrl_MouseMove). <br/>
        ///     Это позволяет осуществлять визуальное перетаскивание контрола курсором мыши.
        /// </summary>
        public static void Add(Control ctrl) {
            ctrl.MouseDown += ctrl_MouseDown;
            ctrl.MouseUp += ctrl_MouseUp;
            ctrl.MouseMove += ctrl_MouseMove;
        }

        /// <summary>
        ///     Метод отцепляет контрол <b> ctrl </b> от своих обработчиков событий (ctrl_MouseDown, ctrl_MouseUp, ctrl_MouseMove). <br/>
        ///     В результате контрол больше нельзя будет перетаскивать курсором мыши.
        /// </summary>
        public static void Remove(Control ctrl) {
            ctrl.MouseDown -= ctrl_MouseDown;
            ctrl.MouseUp -= ctrl_MouseUp;
            ctrl.MouseMove -= ctrl_MouseMove;
        }

        private static void ctrl_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            var ctrl = (sender as Control);
            ctrl.Cursor = oldCursor;
        }

        private static void ctrl_MouseMove(object sender, MouseEventArgs e) {
            var ctrl = sender as Control;
            if (ChangeCursor) {
                if ((e.X >= ctrl.Width - ResizingMargin) && (e.Y >= ctrl.Height - ResizingMargin) && AllowResize)
                    ctrl.Cursor = Cursors.SizeNWSE;
                else if (AllowMove) ctrl.Cursor = Cursors.SizeAll; else ctrl.Cursor = Cursors.Default;
            }
 
            if (e.Button != MouseButtons.Left) return;
 
            var l = ctrl.PointToScreen(e.Location);
            var dx = l.X - startMouse.X;
            var dy = l.Y - startMouse.Y;
 
            if (Math.Max(Math.Abs(dx), Math.Abs(dy)) > 1) {
                if (resizing) {
                    if (AllowResize) {
                        ctrl.Size = new Size(Math.Max(MinSize, startSize.Width + dx), Math.Max(MinSize, startSize.Height + dy));
                        ctrl.Cursor = Cursors.SizeNWSE;
                        if (BringToFront) ctrl.BringToFront();
                    }
                } else {
                    if (AllowMove) {
                        Point newLoc = startLocation + new Size(dx, dy);
                        //запрет на перенос объекта за левую границу экрана
                        if (newLoc.X < 0) newLoc = new Point(0, newLoc.Y);
                        //запрет на перенос объекта за верхнюю границу экрана
                        if (newLoc.Y < 0) newLoc = new Point(newLoc.X, 0);
                        //запрет на перенос объекта за правую границу экрана
                        if (newLoc.X + ctrl.Width > Global.MainWindowWidth) newLoc = new Point(Global.MainWindowWidth - ctrl.Width, newLoc.Y);
                        //запрет на перенос объекта за нижнюю границу экрана
                        if (newLoc.Y + ctrl.Height > Global.MainWindowHeight) newLoc = new Point(newLoc.X, Global.MainWindowHeight - ctrl.Height);
                        ctrl.Location = newLoc;
                        ctrl.Cursor = Cursors.SizeAll;
                        if (BringToFront) ctrl.BringToFront();
                    }
                }
            }
        }

        private static void ctrl_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            var ctrl = sender as Control;
 
            resizing = (e.X >= ctrl.Width - ResizingMargin) && (e.Y >= ctrl.Height - ResizingMargin) && AllowResize;
            startSize = ctrl.Size;
            startMouse = ctrl.PointToScreen(e.Location);
            startLocation = ctrl.Location;
            oldCursor = ctrl.Cursor;
        }
    }
