using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Torpedo;

/* The purpose of this file is to show you all how to have multiple files hold the code
 * for a common class.  What you do is create a new file as a brand new class, with the
 * file name you desire.  You can leave its initial class as Class1.cs, because you will
 * be deleting that.  After you get the new file in your IDE, change its class definition
 * to:
        public partial class MainWindow : Window
 * 
 * "Partial" classes are how WPF lets you spread code for the same class over multipl files.
 * Consider you original MainWindow.xaml.cs file.  It starts with a partial class.  This is
 * because it shares a code file with MainWindow.xaml! */
namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
    }
}