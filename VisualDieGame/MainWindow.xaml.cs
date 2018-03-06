using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;
using Jitter.Collision;
using Jitter;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Jitter.Dynamics.Constraints;

namespace VisualDieGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public sealed class Conversion
        {
            public static float[] ToFloat(JVector vector)
            {
                return new float[4] { vector.X, vector.Y, vector.Z, 0.0f };
            }

            public static float[] ToFloat(JMatrix matrix)
            {
                return new float[12] { matrix.M11, matrix.M21, matrix.M31, 0.0f,
                                   matrix.M12, matrix.M22, matrix.M32, 0.0f,
                                   matrix.M13, matrix.M23, matrix.M33, 1.0f };
            }
        }

        SharpGL.WPF.OpenGLControl OpenGLWindow = new SharpGL.WPF.OpenGLControl();
        RigidBody body;
        RigidBody ground
        CollisionSystem collision;
        World world;

        public MainWindow()
        {
            InitializeComponent();

            collision = new CollisionSystemSAP();
            world = new World(collision);
            Jitter.Collision.Shapes.Shape shape = new BoxShape(1.0f, 1.0f, 1.0f);
            body = new RigidBody(shape);

            JVector pos = new JVector() { X = 0.0f, Y = 0.0f, Z = -12.0f };
            body.Position = pos;

            world.AddBody(body);

            Jitter.Collision.Shapes.Shape groundShape = new BoxShape(100f, 1.0f, 100f);
            ground = new RigidBody(groundShape);
            JVector groundPos = new JVector() {X=0.0f, Y=-2.0f, Z=0.0f };
            ground.Position = groundPos;
            world.AddBody(ground);

            JVector vector = new JVector() { X = 0.0f, Y = -300.0f, Z = 0.0f };
            world.Gravity = vector;
        }

        private void setTransform(OpenGL gl, float[] pos, float[] R)
        {
            //GLfloat
            float[] matrix = new float[16];
            matrix[0] = R[0];
            matrix[1] = R[4];
            matrix[2] = R[8];
            matrix[3] = 0;
            matrix[4] = R[1];
            matrix[5] = R[5];
            matrix[6] = R[9];
            matrix[7] = 0;
            matrix[8] = R[2];
            matrix[9] = R[6];
            matrix[10] = R[10];
            matrix[11] = 0;
            matrix[12] = pos[0];
            matrix[13] = pos[1];
            matrix[14] = pos[2];
            matrix[15] = 1;
            gl.MultMatrix(matrix);
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            world.Step(1.0f / 100.0f, true);

            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Move Left And Into The Screen
            gl.LoadIdentity();

            float[] position = Conversion.ToFloat(body.Position);
            float[] orientation = Conversion.ToFloat(body.Orientation);

            gl.PushMatrix();
            setTransform(gl, position, orientation);

            //Teapot tp = new Teapot();
            //tp.Draw(gl, 14, 1, OpenGL.GL_FILL);
            Cube cube = new Cube();
            var faces = cube.Faces;
            foreach(var face in faces)
            {
                SharpGL.SceneGraph.Assets.Material material = new SharpGL.SceneGraph.Assets.Material();
                material.Ambient = System.Drawing.Color.Crimson;
                face.Material = material;
            }

            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            setTransform(gl, position, orientation);

            //Teapot tp = new Teapot();
            //tp.Draw(gl, 14, 1, OpenGL.GL_FILL);
            Cube cube = new Cube();
            var faces = cube.Faces;
            foreach (var face in faces)
            {
                SharpGL.SceneGraph.Assets.Material material = new SharpGL.SceneGraph.Assets.Material();
                material.Ambient = System.Drawing.Color.Crimson;
                face.Material = material;
            }

            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }
    }
}
