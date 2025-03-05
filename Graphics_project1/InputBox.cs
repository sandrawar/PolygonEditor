using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_project1
{
    public partial class InputBox : Form
    {
        private TextBox textBox;
        private Button submitButton;
        public string InputText { get; private set; }

        public InputBox(string title, string prompt)
        {
            // Ustawienia okna dialogowego
            this.Text = title;
            this.Size = new System.Drawing.Size(350, 200);

            // Tworzenie etykiety
            Label promptLabel = new Label();
            promptLabel.Text = prompt;
            promptLabel.Location = new System.Drawing.Point(10, 10);
            promptLabel.AutoSize = true;

            // Tworzenie TextBox (pole tekstowe)
            textBox = new TextBox();
            textBox.Location = new System.Drawing.Point(10, 40);
            textBox.Width = 250;

            // Tworzenie przycisku
            submitButton = new Button();
            submitButton.Text = "OK";
            submitButton.Size = new System.Drawing.Size(50, 50);
            submitButton.Location = new System.Drawing.Point(10, 70);
            submitButton.Click += new EventHandler(OnSubmit);

            // Dodanie elementów do okna dialogowego
            this.Controls.Add(promptLabel);
            this.Controls.Add(textBox);
            this.Controls.Add(submitButton);
        }

        // Zdarzenie kliknięcia przycisku
        private void OnSubmit(object sender, EventArgs e)
        {
            // Przechwycenie wprowadzonego tekstu i zamknięcie okna
            InputText = textBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Statyczna metoda do wyświetlania InputBox
        public static string Show(string title, string prompt)
        {
            using (InputBox inputBox = new InputBox(title, prompt))
            {
                if (inputBox.ShowDialog() == DialogResult.OK)
                {
                    return inputBox.InputText; // Zwróć tekst wprowadzony przez użytkownika
                }
                return null;
            }
        }
    }
}
