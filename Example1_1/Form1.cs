using OpenTK;
using System;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;

namespace Example1_1
{
    public partial class Form1 : Form
    {
        private bool useLighting = true;
        private bool smoothShading = true;

        private float cubeRotationX = 0f;
        private float cubeRotationY = 0f;
        private float octahedronRotationX = 0f;
        private float octahedronRotationY = 0f;
        private float ixohedronRotationX = 0f;
        private float ixohedronRotationY = 0f;

        private float boundarySize = 4.0f;
        private float aspectRatio = 1.0f;

        private float rotationX = 0f;
        private float rotationY = 0f;
        private int d = 5;
        private float lightRotation = (float)(Math.PI / 2);

        Random rand = new Random();

        float cubeRadius = 1.73f;
        float octahedronRadius = 1.0f;
        float ixahedronRadius = 1.1756f;

        float cubeX = -2.5f;
        float cubeY = 0.0f;
        float cubeZ = 0.0f;
        float octahedronX = 0.0f;
        float octahedronY = 0.0f;
        float octahedronZ = 0.0f;
        float ixahedronX = 2.5f;
        float ixahedronY = 0.0f;
        float ixahedronZ = 0.0f;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            glControl1.Width = this.Width - 30;
            glControl1.Height = this.Height - 60;

            aspectRatio = (float)glControl1.Width / (float)glControl1.Height;

            GL.ClearColor(0.15f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0); // Основной источник (вращающийся)
            GL.Enable(EnableCap.Light1); // Дополнительный источник
            GL.Enable(EnableCap.Normalize);

            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.ColorMaterial);

            float[] globalAmbient = { 0.4f, 0.4f, 0.4f, 1.0f };
            GL.LightModel(LightModelParameter.LightModelAmbient, globalAmbient);

            if (smoothShading)
                GL.ShadeModel(ShadingModel.Smooth);
            else
                GL.ShadeModel(ShadingModel.Flat);

            SetupLights();
            SetupProjection();

            SetupObjects();
        }

        private void SetupObjects()
        {
            cubeX = rand.Next(-30, 30) / 10;
            cubeY = rand.Next(-30, 30) / 10;
            cubeZ = rand.Next(-30, 30) / 10;

            do
            {
                octahedronX = rand.Next(-30, 30) / 10f;
                octahedronY = rand.Next(-30, 30) / 10f;
                octahedronZ = rand.Next(-30, 30) / 10f;
            } while (CheckIntersection(cubeX, cubeY, cubeZ, cubeRadius,
                           octahedronX, octahedronY, octahedronZ, octahedronRadius));

            // Генерация координат икосаэдра с проверкой на пересечение с кубом и октаэдром
            do
            {
                ixahedronX = rand.Next(-30, 30) / 10f;
                ixahedronY = rand.Next(-30, 30) / 10f;
                ixahedronZ = rand.Next(-30, 30) / 10f;
            } while (CheckIntersection(cubeX, cubeY, cubeZ, cubeRadius,
                                       ixahedronX, ixahedronY, ixahedronZ, ixahedronRadius) ||
                     CheckIntersection(octahedronX, octahedronY, octahedronZ, octahedronRadius,
                                       ixahedronX, ixahedronY, ixahedronZ, ixahedronRadius));
        }

        private bool CheckIntersection(float x1, float y1, float z1, float radius1,
                                float x2, float y2, float z2, float radius2)
        {
            float dx = x1 - x2;
            float dy = y1 - y2;
            float dz = z1 - z2;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            return distance < (radius1 + radius2);
        }

        /// <summary>
        /// Настройка проекции камеры с учетом соотношения сторон
        /// </summary>
        private void SetupProjection()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            aspectRatio = (float)glControl1.Width / glControl1.Height;

            float viewSize = 5.5f;

            if (aspectRatio >= 1.0f)
            {
                GL.Ortho(-viewSize * aspectRatio, viewSize * aspectRatio,
                        -viewSize, viewSize,
                        0.1f, 100.0f);
            }
            else
            {
                GL.Ortho(-viewSize, viewSize,
                        -viewSize / aspectRatio, viewSize / aspectRatio,
                        0.1f, 100.0f);
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        /// <summary>
        /// Настройка источников света
        /// </summary>
        private void SetupLights()
        {
            float[] light0Pos = { 0.0f, 4.0f, 0.0f, 1.0f };
            float[] light0Ambient = { 0.35f, 0.3f, 0.2f, 1.0f }; 
            float[] light0Diffuse = { 1.0f, 0.95f, 0.8f, 1.0f };  
            float[] light0Specular = { 1.0f, 0.9f, 0.7f, 1.0f };   

            GL.Light(LightName.Light0, LightParameter.Position, light0Pos);
            GL.Light(LightName.Light0, LightParameter.Ambient, light0Ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, light0Diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, light0Specular);
            GL.Light(LightName.Light0, LightParameter.ConstantAttenuation, 1.0f);
            GL.Light(LightName.Light0, LightParameter.LinearAttenuation, 0.05f);
            GL.Light(LightName.Light0, LightParameter.QuadraticAttenuation, 0.01f);

            // Дополнительный источник (фиксированный)
            float[] light1Pos = { -3.0f, 4.0f, -2.0f, 1.0f };
            float[] light1Ambient = { 0.15f, 0.1f, 0.1f, 1.0f };
            float[] light1Diffuse = { 0.5f, 0.3f, 0.3f, 1.0f };
            float[] light1Specular = { 0.6f, 0.5f, 0.4f, 1.0f };

            GL.Light(LightName.Light1, LightParameter.Position, light1Pos);
            GL.Light(LightName.Light1, LightParameter.Ambient, light1Ambient);
            GL.Light(LightName.Light1, LightParameter.Diffuse, light1Diffuse);
            GL.Light(LightName.Light1, LightParameter.Specular, light1Specular);
        }

        /// <summary>
        /// Вычисление нормали к плоскости по трем вершинам
        /// </summary>
        private Vector3 CalculateNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;
            Vector3 normal = Vector3.Cross(side1, side2);
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// Рисование границ области отображения
        /// </summary>
        private void DrawBoundary()
        {
            GL.Disable(EnableCap.Lighting);
            float size = boundarySize;
            GL.LineWidth(1.5f);
            GL.Color3(0.5f, 0.3f, 0.3f);

            GL.Begin(PrimitiveType.Lines);
            // Рисуем куб-границу
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        GL.Vertex3(i * size, j * size, k * size);
                        GL.Vertex3(i * size, j * size, -k * size);
                        GL.Vertex3(i * size, j * size, k * size);
                        GL.Vertex3(i * size, -j * size, k * size);
                        GL.Vertex3(i * size, j * size, k * size);
                        GL.Vertex3(-i * size, j * size, k * size);
                    }
                }
            }
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }

        ///<summary>
        ///Рисование икосаэдра с освещением
        /// </summary>
        private void DrawIxosahedronWithLight()
        {
            // вершины
            // первый прямоугольник
            Vector3 A = new Vector3(0.0f, 0.618f, 1.0f);
            Vector3 B = new Vector3(0.0f, 0.618f, -1.0f);
            Vector3 C = new Vector3(0.0f, -0.618f, -1.0f);
            Vector3 D = new Vector3(0.0f, -0.618f, 1.0f);
            //второй прямоугольник
            Vector3 E = new Vector3(1.0f, 0.0f, 0.618f);
            Vector3 F = new Vector3(-1.0f, 0.0f, 0.618f);
            Vector3 G = new Vector3(-1.0f, 0.0f, -0.618f);
            Vector3 H = new Vector3(1.0f, 0.0f, -0.618f);
            //третий прямоугольник 
            Vector3 I = new Vector3(0.618f, 1.0f, 0.0f);
            Vector3 J = new Vector3(-0.618f, 1.0f, 0.0f);
            Vector3 K = new Vector3(-0.618f, -1.0f, 0.0f);
            Vector3 L = new Vector3(0.618f, -1.0f, 0.0f);

            // НАСТРОЙКА МАТЕРИАЛА - РАСКАЛЕННЫЙ МЕТАЛЛ
            float[] matAmbient = { 0.5f, 0.1f, 0.05f, 1.0f };
            float[] matDiffuse = { 1.0f, 0.3f, 0.1f, 1.0f };   // Насыщенный красный
            float[] matSpecular = { 1.0f, 0.8f, 0.4f, 1.0f };  // Ярко-золотистые блики
            float[] matEmission = { 0.2f, 0.05f, 0.0f, 1.0f }; // Свечение
            float matShininess = 80.0f; // Очень блестящий

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, matAmbient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, matDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, matSpecular);
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, matEmission);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, matShininess);

            GL.Begin(PrimitiveType.Triangles);
            // Верхняя часть (вокруг вершины A и I, J)
            DrawTriangleWithNormal(A, I, J);
            DrawTriangleWithNormal(A, E, I);
            DrawTriangleWithNormal(A, D, E);
            DrawTriangleWithNormal(A, F, D);
            DrawTriangleWithNormal(A, J, F);

            // Нижняя часть (5 граней)
            DrawTriangleWithNormal(C, L, K);
            DrawTriangleWithNormal(C, K, G);
            DrawTriangleWithNormal(C, G, B);
            DrawTriangleWithNormal(C, B, H);
            DrawTriangleWithNormal(C, H, L);

            // Пояс (10 граней)
            DrawTriangleWithNormal(B, J, I);   // B-I-J ** 
            DrawTriangleWithNormal(B, I, H);   // B-H-I (исправлено: B-H-I) **
            DrawTriangleWithNormal(H, I, E);   // H-E-I (исправлено: H-E-I) ** 
            DrawTriangleWithNormal(E, L, H);   // E-H-L **
            DrawTriangleWithNormal(L, E, D);   // L-D-E (исправлено: L-D-E)
            DrawTriangleWithNormal(D, K, L);   // D-K-L
            DrawTriangleWithNormal(D, F, K);   // D-F-K
            DrawTriangleWithNormal(F, G, K);   // F-G-K
            DrawTriangleWithNormal(F, J, G);   // F-J-G (исправлено: F-J-G)
            DrawTriangleWithNormal(J, B, G);   // J-B-G (исправлено: J-B-G)

            GL.End();
        }

        /// <summary>
        /// Рисование куба с освещением
        /// </summary>
        private void DrawCubeWithLighting()
        {
            // НАСТРОЙКА МАТЕРИАЛА КУБА - ЗОЛОТО
            float[] matAmbient = { 0.3f, 0.22f, 0.1f, 1.0f };
            float[] matDiffuse = { 0.8f, 0.6f, 0.2f, 1.0f };
            float[] matSpecular = { 1.0f, 0.9f, 0.5f, 1.0f };
            float[] matEmission = { 0.05f, 0.03f, 0.0f, 1.0f };
            float matShininess = 85.0f;

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, matAmbient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, matDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, matSpecular);
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, matEmission);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, matShininess);

            float size = 1.0f;

            // РИСОВАНИЕ ГРАНЕЙ КУБА
            GL.Begin(PrimitiveType.Quads);

            // Передняя грань (Z+)
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.Color3(1.0f, 0.5f, 0.5f);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(-size, size, size);

            // Задняя грань (Z-)
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.Color3(0.5f, 1.0f, 0.5f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, -size, -size);

            // Верхняя грань (Y+)
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Color3(0.5f, 0.5f, 1.0f);
            GL.Vertex3(-size, size, -size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, size, -size);

            // Нижняя грань (Y-)
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.Color3(1.0f, 1.0f, 0.5f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, -size, size);
            GL.Vertex3(-size, -size, size);

            // Правая грань (X+)
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.Color3(1.0f, 0.5f, 1.0f);
            GL.Vertex3(size, -size, -size);
            GL.Vertex3(size, size, -size);
            GL.Vertex3(size, size, size);
            GL.Vertex3(size, -size, size);

            // Левая грань (X-)
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.Color3(0.5f, 1.0f, 1.0f);
            GL.Vertex3(-size, -size, -size);
            GL.Vertex3(-size, -size, size);
            GL.Vertex3(-size, size, size);
            GL.Vertex3(-size, size, -size);

            GL.End();
        }

        private void DrawOctahedronWithLighting()
        {
            Vector3 top = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottom = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 front = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 back = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 right = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 left = new Vector3(-1.0f, 0.0f, 0.0f);


            // НАСТРОЙКА МАТЕРИАЛА - РУБИНОВОЕ СТЕКЛО
            float[] matAmbient = { 0.35f, 0.08f, 0.08f, 0.85f };
            float[] matDiffuse = { 0.85f, 0.2f, 0.15f, 0.8f };
            float[] matSpecular = { 1.0f, 0.95f, 0.9f, 1.0f };
            float[] matEmission = { 0.08f, 0.0f, 0.0f, 1.0f };
            float matShininess = 120.0f;  

            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, matAmbient);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, matDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, matSpecular);
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, matEmission);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, matShininess);

            GL.Begin(PrimitiveType.Triangles);

            // Верхняя пирамида
            DrawTriangleWithNormal(top, front, right);
            DrawTriangleWithNormal(top, right, back);
            DrawTriangleWithNormal(top, back, left);
            DrawTriangleWithNormal(top, left, front);

            // Нижняя пирамида
            DrawTriangleWithNormal(bottom, right, front);
            DrawTriangleWithNormal(bottom, back, right);
            DrawTriangleWithNormal(bottom, left, back);
            DrawTriangleWithNormal(bottom, front, left);

            GL.End();
        }

        private void DrawTriangleWithNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 normal = CalculateNormal(v1, v2, v3);
            GL.Normal3(normal);
            GL.Vertex3(v1);
            GL.Vertex3(v2);
            GL.Vertex3(v3);
        }

        private void DrawLightSource()
        {
            GL.Disable(EnableCap.Lighting);
            GL.PushMatrix();
            
            float radius1 = 5.0f;
            GL.Translate(0,  // X координата
                radius1 * (float)Math.Sin(lightRotation),
                radius1 * (float)Math.Cos(lightRotation));  // Z координата);

            GL.Color3(1.0f, 1.0f, 0.7f);

            float radius = 0.3f;
            int segments = 12;

            for (int i = 0; i < segments; i++)
            {
                float theta1 = i * 2.0f * (float)Math.PI / segments;
                float theta2 = (i + 1) * 2.0f * (float)Math.PI / segments;

                GL.Begin(PrimitiveType.TriangleStrip);
                for (int j = 0; j <= segments; j++)
                {
                    float phi = j * (float)Math.PI / segments;

                    float x = radius * (float)Math.Sin(phi) * (float)Math.Cos(theta1);
                    float y = radius * (float)Math.Cos(phi);
                    float z = radius * (float)Math.Sin(phi) * (float)Math.Sin(theta1);
                    GL.Vertex3(x, y, z);

                    x = radius * (float)Math.Sin(phi) * (float)Math.Cos(theta2);
                    y = radius * (float)Math.Cos(phi);
                    z = radius * (float)Math.Sin(phi) * (float)Math.Sin(theta2);
                    GL.Vertex3(x, y, z);
                }
                GL.End();
            }

            GL.PopMatrix();
            GL.Enable(EnableCap.Lighting);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Установка камеры
            GL.LoadIdentity();
            GL.Translate(0.0f, 0.0f, -15.0f);

            // Поворот всей сцены
            GL.Rotate(rotationX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(rotationY, 0.0f, 1.0f, 0.0f);

            // Рисование границ области
            DrawBoundary();

            // Рисование куба слева
            GL.PushMatrix();
            GL.Translate(cubeX, cubeY, cubeZ);
            GL.Rotate(cubeRotationX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(cubeRotationY, 0.0f, 1.0f, 0.0f);
            DrawCubeWithLighting();
            GL.PopMatrix();

            // Рисование октаэдра по центру
            GL.PushMatrix();
            GL.Translate(octahedronX, octahedronY, octahedronZ);
            GL.Rotate(octahedronRotationX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(octahedronRotationY, 0.0f, 1.0f, 0.0f);
            DrawOctahedronWithLighting();
            GL.PopMatrix();

            // Рисование пентагональной икосаэдра справа
            GL.PushMatrix();
            GL.Translate(ixahedronX, ixahedronY, ixahedronZ);
            GL.Rotate(ixohedronRotationX, 1.0f, 0.0f, 0.0f);
            GL.Rotate(ixohedronRotationY, 0.0f, 1.0f, 0.0f);
            //DrawPentagonalBipyramidWithLighting();
            DrawIxosahedronWithLight();
            GL.PopMatrix();

            // Рисование источника света
            DrawLightSource();

            // Завершение отрисовки
            GL.Flush();
            GL.Finish();
            glControl1.SwapBuffers();
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // УПРАВЛЕНИЕ ПОВОРОТОМ СЦЕНЫ
                case Keys.W:
                    rotationX += d;
                    UpdateLightPosition();
                    break;
                case Keys.S:
                    rotationX -= d;
                    UpdateLightPosition();
                    break;
                case Keys.D:
                    rotationY += d;
                    UpdateLightPosition();
                    break;
                case Keys.A:
                    rotationY -= d;
                    UpdateLightPosition();
                    break;

                // УПРАВЛЕНИЕ ИСТОЧНИКОМ СВЕТА (вращение в плоскости ZX)
                case Keys.Q:
                    lightRotation += 0.05f;
                    UpdateLightPosition();
                    break;
                case Keys.E:
                    lightRotation -= 0.05f;
                    UpdateLightPosition();
                    break;

                // УПРАВЛЕНИЕ ПОВОРОТОМ куба
                case Keys.NumPad8:
                    cubeRotationX += d;
                    break;
                case Keys.NumPad2:
                    cubeRotationX -= d;
                    break;
                case Keys.NumPad6:
                    cubeRotationY += d;
                    break;
                case Keys.NumPad4:
                    cubeRotationY -= d;
                    break;

                // УПРАВЛЕНИЕ ПОВОРОТОМ ОКТАЭДРА
                case Keys.T:
                    octahedronRotationX += d;
                    break;
                case Keys.G:
                    octahedronRotationX -= d;
                    break;
                case Keys.H:
                    octahedronRotationY += d;
                    break;
                case Keys.F:
                    octahedronRotationY -= d;
                    break;

                // УПРАВЛЕНИЕ ПОВОРОТОМ икосаэдра
                case Keys.I:
                    ixohedronRotationX += d;
                    break;
                case Keys.K:
                    ixohedronRotationX -= d;
                    break;
                case Keys.L:
                    ixohedronRotationY += d;
                    break;
                case Keys.J:
                    ixohedronRotationY -= d;
                    break;

                // СБРОС ВСЕХ ПОВОРОТОВ
                case Keys.R:
                    rotationX = 0;
                    rotationY = 0;
                    cubeRotationX = 0;
                    cubeRotationY = 0;
                    octahedronRotationX = 0;
                    octahedronRotationY = 0;
                    ixohedronRotationX = 0;
                    ixohedronRotationY = 0;
                    lightRotation = (float)(Math.PI / 2);
                    UpdateLightPosition();
                    break;
            }

            glControl1.Invalidate();
        }

        private void UpdateLightPosition()
        {

            float radius = 5.0f;
            //float yPosition = 2.5f;

            float[] light0Pos = {
                0,  // X координата
                radius * (float)Math.Sin(lightRotation),   
                radius * (float)Math.Cos(lightRotation),  // Z координата
                1.0f
             };
            GL.Light(LightName.Light0, LightParameter.Position, light0Pos);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            SetupProjection();
            glControl1.Invalidate();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (glControl1 != null)
            {
                glControl1.Width = this.ClientSize.Width - 20;
                glControl1.Height = this.ClientSize.Height - 20;

                glControl1.Location = new System.Drawing.Point(10, 10);
            }
        }
    }
}
