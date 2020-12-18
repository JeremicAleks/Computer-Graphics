using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.Globalization;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;


        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {

                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\bike"), "Downloaded Object.obj", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\sem"), "semaforSimple.3ds", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);

            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);

        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);

        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.E:
                    if (World.startAnimation == false)
                    {
                        if (m_world.RotationX >= 0f && m_world.RotationX <= 180f)
                        {
                            m_world.RotationX -= 5.0f;
                        }
                        else if (m_world.RotationX < 0)
                        {
                            m_world.RotationX = 0;
                        }
                        else if (m_world.RotationX > 180)
                        {
                            m_world.RotationX = 180;
                        }
                    }
                    break;
                case Key.D:
                    if (World.startAnimation == false)
                    {
                        if (m_world.RotationX >= 0f && m_world.RotationX <= 180f)
                        {
                            m_world.RotationX += 5.0f;
                        }
                        else if (m_world.RotationX < 0)
                        {
                            m_world.RotationX = 0;
                        }
                        else if (m_world.RotationX > 180)
                        {
                            m_world.RotationX = 180;
                        }
                    }
                    break;
                case Key.S:
                    if (World.startAnimation == false)
                    {
                        m_world.RotationY -= 5.0f;
                    } break;
                case Key.F:
                    if (World.startAnimation == false)
                    {
                        m_world.RotationY += 5.0f;
                    } break;
                case Key.Add:
                    if (World.startAnimation == false)
                    {
                        m_world.SceneDistance -= 700.0f;
                    }
                    break;
                case Key.Subtract:
                    if (World.startAnimation == false)
                    {
                        m_world.SceneDistance += 700.0f;
                    }
                    break;
                case Key.V:
                    if (World.startAnimation)
                    {
                        World.timer.Stop();
                        World.timer = null;
                        World.startAnimation = false;
                    }
                    else
                    {
                        World.startAnimation = true;
                        World.timer = new System.Windows.Threading.DispatcherTimer();
                        World.timer.Tick += new EventHandler(World.Animacija);
                        World.timer.Interval = TimeSpan.FromMilliseconds(1);
                        World.timer.Start();
                    }




                    break;
            }
        }

        private void Sl_bandera_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Console.WriteLine(sl_bandera.Value);
            if (m_world != null)
            {
                m_world.Skaliranje_bandere = sl_bandera.Value;
            }
        }

        private void Btn_Ambient_Click(object sender, RoutedEventArgs e)
        {

            if(validation(AmbijentalnaR)  && validation(AmbijentalnaG) && validation(AmbijentalnaB)
                && validation(AmbijentalnaAlpha))
            {
                m_world.Ambient_R = float.Parse(AmbijentalnaR.Text, CultureInfo.InvariantCulture.NumberFormat);
                m_world.Ambient_G= float.Parse(AmbijentalnaG.Text, CultureInfo.InvariantCulture.NumberFormat);
                m_world.Ambient_B= float.Parse(AmbijentalnaB.Text, CultureInfo.InvariantCulture.NumberFormat);
                m_world.Ambient_Alpha= float.Parse(AmbijentalnaAlpha.Text, CultureInfo.InvariantCulture.NumberFormat);
                AmbijentalnaR.Text = "";AmbijentalnaG.Text = ""; AmbijentalnaB.Text = ""; AmbijentalnaAlpha.Text = ""; 
            }

        }

        private Boolean validation(TextBox tb)
        {

            
                try
                {
                    float valueG = float.Parse(tb.Text);
                    if (valueG < 0 || valueG > 1)
                    {
                        MessageBox.Show("Za vrednost komponenti "+ tb.Name +" se mora zadati broj izmedju 0 i 1!", "Greška: Ambijentalna komponenta");
                        tb.Text = "";
                    return false;
                    }
                }
                catch
                {
                    MessageBox.Show("Za vrednost komponente " + tb.Name +" se mora zadati broj!", "Greška: ambijentalna komponenta");
                tb.Text = "";
                return false;
                }
                

                return true;

        }

        private void Sl_brzina_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
               World.brzina_motora=(float)sl_brzina.Value;
        }
    }
    
}
