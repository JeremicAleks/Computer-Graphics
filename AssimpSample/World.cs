// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright></copyright>
// <author>Aleksandar Jeremic</author>
// <summary>Projekat Grafika 12.1</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi


        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene_motor;
        private AssimpScene m_scene_semafor;
        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;
        /// <summary>
        ///	 Menjanje visine bandere preko slajdera
        /// </summary>
        private double skaliranje_bandere = 1;

        /// <summary>
        ///	 Menjanej ambinetalne komponente tackastog izvora svetlosti
        /// </summary>
        private float m_ambient_R= 1.0f;
        private float m_ambient_G = 1.0f;
        private float m_ambient_B= 0.7f;
        private float m_ambient_Alpha = 1.0f;


        /// <summary>
        ///	 Atributi za animaciju.
        /// </summary>
        public static bool startAnimation = false;
        public static DispatcherTimer timer;
        private static float x=0.0f;
        private static float z=3500.0f;
        private static float rotate=0.0f;
        public static bool endAnimation=false;
        public static bool disapper = false;

        /// <summary>
        /// Pomeranje brzine motora
        /// </summary>
        public static float brzina_motora = 100.0f;


        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 12000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Road = 0, Wood, Building };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//Texture//road.jpg", "..//..//Texture//wood.jpg", "..//..//Texture//building.jpg" };

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Motor_Scene
        {
            get { return m_scene_motor; }
            set { m_scene_motor = value; }
        }

        public AssimpScene Semafor_Scene
        {
            get { return m_scene_semafor; }
            set { m_scene_semafor = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }
        /// <summary>
        /// Pocetak Animacije
        /// </summary>
        //public bool StartAnimation
        //{
        //    get { return startAnimation; }
        //    set { startAnimation = value; }
        //}
        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public double Skaliranje_bandere
        {
            get { return skaliranje_bandere; }
            set { skaliranje_bandere = value; }
        }

        public float Ambient_R {
            get { return m_ambient_R;  }
            set { m_ambient_R = value; }
        }

        public float Ambient_G
        {
            get { return m_ambient_G; }
            set { m_ambient_G = value; }
        }


        public float Ambient_B
        {
            get { return m_ambient_B; }
            set { m_ambient_B = value; }
        }
        public float Ambient_Alpha
        {
            get { return m_ambient_Alpha; }
            set { m_ambient_Alpha = value;
            }
        }


        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, String scenePath2, String sceneFileName2, int width, int height, OpenGL gl)
        {
            this.m_scene_motor = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene_semafor = new AssimpScene(scenePath2, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);

            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_NORMALIZE);

            //STAVKA 1
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT);
            gl.ColorMaterial(OpenGL.GL_BACK, OpenGL.GL_DIFFUSE);

            Tekstura(gl);

            m_scene_motor.LoadScene();
            m_scene_motor.Initialize();

            m_scene_semafor.LoadScene();
            m_scene_semafor.Initialize();
        }


        //STAVKA 3
        /// <summary>
        /// Podesavanje teksture
        /// </summary>
         private void Tekstura(OpenGL gl)
         {
            // Teksture se primenjuju sa parametrom add
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);

            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);

                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);		// Najblizi sused filtriranja
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);     // Najblizi sused filtriranja
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);          //Podesen wrapping na repeat 
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);                //Nacin stapanja teksture sa materijalom 

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        //STAVKA 2

        

            /// <summary>
            /// Definisanje tackastog izvora svetlosti. STAVKA 2
            /// </summary>
        private void SvetloIzvor(OpenGL gl,float r,float g,float b,float a) {
            
            float[] light0pos = new float[] { 10000.0f, 500.0f, 10000, 1.0f };
            float[] light0ambient= { r, g, b, a };
            float[] light0diffuse = { r, g, b, a };
            
            // Pridruži komponente svetlosnom izvoru 0
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            // Podesi parametre tackastog svetlosnog izvora

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
          
        }
        /// <summary>
        /// Stavka 9 svetlo usmereno ka motoru
        /// </summary>
        /// <param name="gl"></param>
        private void crvenoSvetlo(OpenGL gl)
        {
            float[] light1pos = new float[] { -2400.0f, 500.0f, 1000, 1.0f };
            //float[] ambijentalnaKomponenta1 = { 1f, 0f, 0f, 1f };
            float[] difuznaKomponenta1 = { 1.0f, 0f, 0f, 1.0f };
            float[] light_dir = { 1f, -0.5f, 0.8f };
            // Pridruži komponente svetlosnom izvoru 1
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE,difuznaKomponenta1);
            // Podesi parametre tackastog svetlosnog izvora
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light_dir);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
        }




        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Viewport(0, 0, m_width, m_height);


            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            // stavka 6 pozicioniranje kamere iza i iznad motora usmerena ka putu
            gl.LookAt(0, 1500, 0, 0, 0, -m_sceneDistance, 0, 1, 0);

            gl.PushMatrix();
            SvetloIzvor(gl,m_ambient_R,m_ambient_G,m_ambient_B,m_ambient_Alpha);
            crvenoSvetlo(gl);
            DrawSemafor(gl);

            DrawMotor(gl);
            gl.PopMatrix();


            DrawPodloga(gl);
           
            DrawZgrada1(gl);
            DrawZgrada2(gl);

            DrawBandera1(gl);

            gl.PushMatrix();
            gl.Scale(1, skaliranje_bandere, 1);
            DrawBandera2(gl);
            gl.PopMatrix();
            
            DrawText(gl);

            gl.PopMatrix();

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.5f, 30000f);
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        public void DrawMotor(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(x, -500.0f, z);
            gl.Rotate(90, 180, rotate);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(2, 2, 2);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE); // Nacin stapanja teksture motora
            m_scene_motor.Draw();
            gl.PopMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

        }

        public void DrawSemafor(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Translate(800.0f, 1500.0f, 2200.0f);
            gl.Color(0.5f, 0.5f, 0.5f, 1.0f);
            //gl.Rotate(0, 0, 0);
            gl.Scale(8, 8, 8);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE); // Nacin stapanja teksture semafora
            m_scene_semafor.Draw();
            gl.PopMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
        }

        private void DrawPodloga(OpenGL gl)
        {

            //STAVKA 5 Podloga tekstura betona
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(100.0f, 100.0f, 100.0f); // skaliranje teksture podloge
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Color(0.1, 0.1, 0.1);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Road]);

            gl.PushMatrix();
            gl.Translate(0, -500f, 0);
            gl.Scale(30, 15, 30);
            gl.Begin(OpenGL.GL_QUADS); 
            gl.TexCoord(0, 0);
            gl.Vertex(300, 0, 200);
            gl.TexCoord(1, 0);
            gl.Vertex(300, 0, -200);
            gl.TexCoord(1, 1);
            gl.Vertex(-300, 0, -200);
            gl.TexCoord(0, 1);
            gl.Vertex(-300, 0, 200);

            gl.End();

            gl.MatrixMode(OpenGL.GL_TEXTURE); // vracanje na pocetnu matricu za teksture
            gl.LoadIdentity();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();

        }

        private void DrawZgrada1(OpenGL gl)
        {
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Building]);
            gl.Scale(2000, 3000, 3000);
            gl.Translate(-3f, 0.84f, 0f);
            //gl.Color(0.57f, 0.28f, 0.0f);
            Cube zgrada1 = new Cube();
            zgrada1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);




            gl.PopMatrix();
        }


        private void DrawZgrada2(OpenGL gl)
        {
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Building]);
            gl.Scale(2000, 3000, 3000);
            gl.Translate(3f, 0.84f, 0f);
            //gl.Color(0.57f, 0.28f, 0.0f);
            Cube zgrada2 = new Cube();
            zgrada2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);




            gl.PopMatrix();
        }

        private void DrawBandera1(OpenGL gl)
        {
            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            //BANDERA TEKSUTRA DRVETA STAVKA 4
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            gl.Translate(-3000.0f, -485.0f, 0.0f);
            gl.Rotate(-90f, 0f, 0f);
            Cylinder stub = new Cylinder();
            stub.TopRadius = 50f;
            stub.BaseRadius = 50f;
            stub.Height = 2500f;
            stub.CreateInContext(gl);
            stub.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            gl.Translate(-3000.0f, 2200.0f, 0.0f);
            gl.Scale(300f, 300f, 300f);

            Cube svetlo = new Cube();
            svetlo.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

        }

        private void DrawBandera2(OpenGL gl)
        {
            gl.PushMatrix();
            //BANDERA TEKSUTRA DRVETA STAVKA 4
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            gl.Translate(3000.0f, -485.0f, 0.0f);
            gl.Rotate(-90f, 0f, 0f);
            Cylinder stub = new Cylinder();
            stub.TopRadius = 50f;
            stub.BaseRadius = 50f;
            stub.Height = 2500f;
            stub.CreateInContext(gl);
            stub.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE_MATRIX);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            //gl.Color(0.45f, 0.47f, 0.44f);
            gl.Translate(3000.0f, 2200.0f, 0.0f);
            gl.Scale(300f, 300f, 300f);

            Cube svetlo = new Cube();
            svetlo.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }


        private void DrawText(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            gl.Color(0f, 0f, 1f);
            gl.DrawText(m_width - 400, 140, 0f, 0f, 1f, "", 10, "");
            gl.DrawText(m_width - 400, 140, 0f, 0f, 1f, "Arial Italic", 10, "Predmet : Racunarska grafika");
            gl.DrawText(m_width - 400, 110, 0f, 0f, 1f, "Arial Italic", 10, "Sk. god: 2018/19");
            gl.DrawText(m_width - 400, 80, 0f, 0f, 1f, "Arial Italic", 10, "Ime: Aleksandar");
            gl.DrawText(m_width - 400, 50, 0f, 0f, 1f, "Arial Italic", 10, "Prezime: Jeremic");
            gl.DrawText(m_width - 400, 20, 0f, 0f, 1f, "Arial Italic", 10, "Sifra zad: 12.1");
            gl.Viewport(0, 0, m_width, m_height);
            gl.PopMatrix();


        }
        /// <summary>
        /// Stavka 11 Animacija klikom na dugme V
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Animacija(object sender, EventArgs e)
        {
            if (z == 3500.0f)
            {
                timer.Start();
                z -= brzina_motora;
            }else if(z<3500 && z > -4000)
            {
                z -= brzina_motora;
            }else if (z<= -4000 && z>=-4300)
            {
                
                if (x == 0)
                {
                    rotate = -90;
                    x += brzina_motora;
                }else if (x>0 && x < 10000)
                {
                    x += brzina_motora;
                }else if (x >= 10000 && x<=10400)
                {
                    disapper = true;
                    x += brzina_motora;
                   
                }else if (x>10000 && x < 11000)
                {
                    x += brzina_motora;
                }else if (x >= 11000)
                {
                    startAnimation = false;
                    endAnimation = true;
                    
                        z = 3500.0f;
                        x = 0.0f;
                        rotate = 0;
                        disapper = false;
                    
                   
                    timer.Stop();

                }
            }
        }



        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene_motor.Dispose();
                m_scene_semafor.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
