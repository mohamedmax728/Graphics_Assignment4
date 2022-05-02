using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using System.Diagnostics;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        uint sbufferid;
        uint mbufferid;
        uint ebufferid;

        vec3 scenter, mcenter, ecenter;
        mat4 modmatrixs, modmatrixe, modmatrixm;
        mat4 viewmatrix, projmatrix;

        int shmodelmatrixid;
        int shviewmatrixid;
        int shprojectionmatrixid;
        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            Gl.glClearColor(0, 0, 0, 1);
            timer.Start();
            float[] sun =
            {
                20.0f,  -5.0f   -5.0f,  1.0f,   1.0f,   0.0f,
                40.0f, -5.0f , -5.0f,   1.0f,   1.0f,   0.0f,
                30.0f, 10.0f, -5.0f,   1.0f,   1.0f,   0.0f
            };

            float[] earth =
            {
                -5.0f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f,
                5.0f, 0.0f, -5.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 10.0f, -5.0f, 0.0f, 0.0f, 1.0f,
            };

            float[] moon =
            {
                -15.0f,  15.0f, -5.0f, 0.5f, 0.5f, 0.5f,
                -20.0f,  15.0f, -5.0f, 0.5f, 0.5f, 0.5f,
                -17.5f,  20.0f, -5.0f, 0.5f, 0.5f, 0.5f,
            };
            sbufferid = GPU.GenerateBuffer(sun);
            ebufferid = GPU.GenerateBuffer(earth);
            mbufferid = GPU.GenerateBuffer(moon);

            scenter = new vec3(30, 0, -5);
            ecenter = new vec3(0, 3.3f, -5);
            mcenter = new vec3(-17.5f, 16.6f, -5);

            modmatrixs = new mat4(1);
            modmatrixe = new mat4(1);
            modmatrixm = new mat4(1);

            viewmatrix = glm.lookAt(
                    new vec3(0, 0, 100),
                    new vec3(0, 0, 0),
                    new vec3(0, 1, 0)

                );
            projmatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);

            sh.UseShader();

            shmodelmatrixid = Gl.glGetUniformLocation(sh.ID, "modelmatrix");
            shviewmatrixid = Gl.glGetUniformLocation(sh.ID, "viewmatrix");
            shprojectionmatrixid = Gl.glGetUniformLocation(sh.ID, "projectationmatrix");

            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixs.to_array());
            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixe.to_array());
            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixm.to_array());
            Gl.glUniformMatrix4fv(shviewmatrixid, 1, Gl.GL_FALSE, viewmatrix.to_array());
            Gl.glUniformMatrix4fv(shprojectionmatrixid, 1, Gl.GL_FALSE, projmatrix.to_array());

        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, sbufferid);
            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixs.to_array());// this value to vertex shader
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, ebufferid);
            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixe.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, mbufferid);
            Gl.glUniformMatrix4fv(shmodelmatrixid, 1, Gl.GL_FALSE, modmatrixm.to_array());
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 3);
            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);

        }
        const float rotspeed = 1f;
        float rotangle = 0;
        Stopwatch timer = Stopwatch.StartNew();
        public void Update()
        {
            timer.Stop();
            var detime = timer.ElapsedMilliseconds / 1000.0f;
            rotangle += rotspeed * detime;

            List<mat4> sunrot = new List<mat4>();
            sunrot.Add(glm.translate(new mat4(1), -1 * scenter));
            sunrot.Add(glm.rotate(rotangle, new vec3(0, 0, 1)));
            sunrot.Add(glm.translate(new mat4(1), scenter));

            modmatrixs = MathHelper.MultiplyMatrices(sunrot);


            List<mat4> earthrot = new List<mat4>();
            earthrot.Add(glm.translate(new mat4(1), -1 * ecenter));
            earthrot.Add(glm.rotate(rotangle, new vec3(0, 0, 1)));
            earthrot.Add(glm.translate(new mat4(1), ecenter));

            earthrot.Add(glm.translate(new mat4(1), -1 * scenter));
            earthrot.Add(glm.rotate(rotangle, new vec3(0, 0, 1)));
            earthrot.Add(glm.translate(new mat4(1), scenter));

            modmatrixe = MathHelper.MultiplyMatrices(earthrot);


            List<mat4> moonrot = new List<mat4>();
            moonrot.Add(glm.translate(new mat4(1), -1 * mcenter));
            moonrot.Add(glm.rotate(rotangle, new vec3(0,0,1)));
            moonrot.Add(glm.translate(new mat4(1),  mcenter));

            moonrot.Add(glm.translate(new mat4(1), -1 * ecenter));
            moonrot.Add(glm.rotate(rotangle, new vec3(0, 0, 1)));
            moonrot.Add(glm.translate(new mat4(1), ecenter));

            moonrot.Add(glm.translate(new mat4(1), -1 * scenter));
            moonrot.Add(glm.rotate(rotangle, new vec3(0, 0, 1)));
            moonrot.Add(glm.translate(new mat4(1), scenter));

            modmatrixm = MathHelper.MultiplyMatrices(moonrot);

            timer.Reset();
            timer.Start();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
