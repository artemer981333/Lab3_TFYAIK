using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        List<DocPage> Pages;
        LocalisationController localisation;
        CodeHandler codeHandler;
        private string CopyBuffer;

        public Form1()
        {
            InitializeComponent();
        }
        private void SaveFile(int pageNumber)
        {
            Pages[pageNumber].Save();
            PagesTab.TabPages[pageNumber].Text = Pages[pageNumber].Title;
        }
        private void SaveAsFile(int pageNumber)
        {
            Pages[pageNumber].SaveAs();
            PagesTab.TabPages[pageNumber].Text = Pages[pageNumber].Title;
        }
        private void UpdateText(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].Text = CodeField.Text;
            UpdateInterface();
        }
        private void CreateClick(object sender, EventArgs e)
        {
            Pages.Add(new DocPage());
            PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
            PagesTab.SelectedIndex = Pages.Count - 1;
        }
        private void OpenClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult res = openFileDialog.ShowDialog();
            if (res == DialogResult.Cancel)
                return;
            try
            {
                Pages.Add(DocPage.OpenFromFile(openFileDialog.FileName));
                PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
            }
            catch (Exception err)
            {
                ResultField.Text = err.Message;
            }
            PagesTab.SelectedIndex = Pages.Count - 1;
        }

        private void SaveClick(object sender, EventArgs e)
        {
            SaveFile(PagesTab.SelectedIndex);
        }

        private void SaveAsClick(object sender, EventArgs e)
        {
            SaveAsFile(PagesTab.SelectedIndex);
        }

        private void ExitClick(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateInterface()
        {
            if (PagesTab.SelectedIndex == -1)
            {
                RepeatButton.Enabled = false;
                CancelButton.Enabled = false;
                RowsNumbers.Text = "";
                return;
            }

            RepeatButton.Enabled = Pages[PagesTab.SelectedIndex].CanRepeat;
            CancelButton.Enabled = Pages[PagesTab.SelectedIndex].CanCancel;

            UpdateRowsNumbers();
            codeHandler.HandleText();
            if (HTTPGETFinder.CheckString(CodeField.Text))
                ResultField.Text = "HTTP GET";
            else
                ResultField.Text = "NOT HTTP GET";
        }

        private void UpdateRowsNumbers()
        {
            string numbers = "";
            int start = CodeField.GetLineFromCharIndex(CodeField.GetCharIndexFromPosition(new Point(0, 0)));
            int end = CodeField.GetLineFromCharIndex(CodeField.GetCharIndexFromPosition(new Point(CodeField.Size.Width, CodeField.Size.Height)));
            for (int i = start; i <= end; i++)
                numbers += i + ":\n";

            RowsNumbers.Text = numbers;
        }

        private void TabChanged(object sender, EventArgs e)
        {
            if (PagesTab.SelectedIndex == -1)
            {
                CodeField.Text = "";
                ResultField.Text = "";
                return;
            }
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
            ResultField.Text = Pages[PagesTab.SelectedIndex].ResultText;
        }

        private void CloseForm(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < Pages.Count; i++)
                if (!Pages[i].Close())
                {
                    e.Cancel = true;
                    return;
                }
        }
        private void CancelClick(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].CancelState();
            int index = CodeField.SelectionStart;
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
        }

        private void RepeatClick(object sender, EventArgs e)
        {
            Pages[PagesTab.SelectedIndex].RepeatState();
            CodeField.Text = Pages[PagesTab.SelectedIndex].Text;
        }

        private void CutClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            int SelectionStart = CodeField.SelectionStart;
            CopyBuffer = CodeField.SelectedText;
            CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
            CodeField.SelectionStart = SelectionStart;
        }

        private void CopyClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            CopyBuffer = CodeField.SelectedText;
        }

        private void PasteClick(object sender, EventArgs e)
        {
            if (CopyBuffer == null)
                return;
            if (CopyBuffer == "")
                return;
            int SelectionStart;
            if(CodeField.SelectionLength != 0)
            {
                SelectionStart = CodeField.SelectionStart;
                CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
                CodeField.SelectionStart = SelectionStart;
            }
            SelectionStart = CodeField.SelectionStart + CopyBuffer.Length;
            CodeField.Text = CodeField.Text.Insert(CodeField.SelectionStart, CopyBuffer);
            CodeField.SelectionStart = SelectionStart;
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (CodeField.SelectionLength == 0)
                return;
            int SelectionStart = CodeField.SelectionStart;
            CodeField.Text = CodeField.Text.Remove(CodeField.SelectionStart, CodeField.SelectionLength);
            CodeField.SelectionStart = SelectionStart;
        }
        
        private void CodeFontUp(object sender, EventArgs e)
        {
            CodeField.Font = new Font(CodeField.Font.FontFamily, Math.Min(CodeField.Font.Size + 1, 20));
        }

        private void CodeFontDown(object sender, EventArgs e)
        {
            CodeField.Font = new Font(CodeField.Font.FontFamily, Math.Max(CodeField.Font.Size - 1, 10));
        }

        private void OutFontUp(object sender, EventArgs e)
        {
            ResultField.Font = new Font(ResultField.Font.FontFamily, Math.Min(ResultField.Font.Size + 1, 20));
        }

        private void OutFontDown(object sender, EventArgs e)
        {
            ResultField.Font = new Font(ResultField.Font.FontFamily, Math.Min(ResultField.Font.Size - 1, 20));
        }

        private void Drop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                for (int i = 0; i < fileNames.Length; i++)
                {
                    try
                    {
                        Pages.Add(DocPage.OpenFromFile(fileNames[i]));
                        PagesTab.TabPages.Add(new TabPage(Pages[Pages.Count - 1].Title));
                    }
                    catch (Exception err)
                    {
                        ResultField.Text = err.Message;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CodeField.AllowDrop = true;
            CodeField.DragDrop += Drop;

            localisation = new LocalisationController("Localisations/Localisations.txt");
            SetLocalisations();
            Locale();

            Pages = new List<DocPage>();
            Pages.Add(new DocPage());
            PagesTab.TabPages.Add(new TabPage(Pages[0].Title));

            CancelButton.Enabled = false;
            RepeatButton.Enabled = false;

            RowsNumbers.Font = CodeField.Font;

            codeHandler = new CodeHandler(CodeField);

            UpdateInterface();
        }

        private void SetLocalisations()
        {
            List<string> loclist = localisation.Localisations;
            foreach (string str in loclist)
            {
                ToolStripMenuItem tmp = new ToolStripMenuItem();
                tmp.Name = str + "LangStrip";
                tmp.Click += SetLocalisation;
                tmp.Text = str;
                LocalisationStrip.DropDownItems.Add(tmp);
            }
        }
        private void Locale()
        {
            FileStrip.Text = localisation["Файл"];
            EditStrip.Text = localisation["Правка"];
            TextStrip.Text = localisation["Текст"];
            PlayStrip.Text = localisation["Пуск"];
            InfoStrip.Text = localisation["Справка"];
            ViewStrip.Text = localisation["Вид"];
            CreateStrip.Text = localisation["Создать"];
            CreateButton.Text = localisation["Создать"];
            OpenStrip.Text = localisation["Открыть"];
            OpenButton.Text = localisation["Открыть"];
            SaveStrip.Text = localisation["Сохранить"];
            SaveButton.Text = localisation["Сохранить"];
            SaveAsStrip.Text = localisation["Сохранить как"];
            ExitStrip.Text = localisation["Выход"];
            CancelStrip.Text = localisation["Отменить"];
            CancelButton.Text = localisation["Отменить"];
            RepeatStrip.Text = localisation["Повторить"];
            RepeatButton.Text = localisation["Повторить"];
            CutStrip.Text = localisation["Вырезать"];
            CutButton.Text = localisation["Вырезать"];
            CopyStrip.Text = localisation["Копировать"];
            CopyButton.Text = localisation["Копировать"];
            PasteStrip.Text = localisation["Вставить"];
            PasteButton.Text = localisation["Вставить"];
            DeleteStrip.Text = localisation["Удалить"];
            SelectAllStrip.Text = localisation["Выделить все"];
            T1Strip.Text = localisation["Постановка задачи"];
            T2Strip.Text = localisation["Грамматика"];
            T3Strip.Text = localisation["Классификация грамматики"];
            T4Strip.Text = localisation["Метод анализа"];
            T5Strip.Text = localisation["Диагностика и нейтрализация ошибок"];
            T6Strip.Text = localisation["Тестовый пример"];
            T7Strip.Text = localisation["Список литературы"];
            T8Strip.Text = localisation["Исходный код программы"];
            CallInfoStrip.Text = localisation["Вызов справки"];
            AboutStrip.Text = localisation["О программе"];
            TextSizeStrip.Text = localisation["Размер текста"];
            CodeFieldStrip.Text = localisation["Окно кода"];
            ResultFieldStrip.Text = localisation["Окно вывода"];
            CodeFontUpStrip.Text = localisation["Увеличить шрифт"];
            CodeFontDownStrip.Text = localisation["Уменьшить шрифт"];
            ResultFontUpStrip.Text = localisation["Увеличить шрифт"];
            ResultFontDownStrip.Text = localisation["Уменьшить шрифт"];
            LocalisationStrip.Text = localisation["Локализация"];

            DocPage.DefaultTitle = localisation["Новый документ"];
        }
        private void About(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\index.html");
        }

        private void SelectAllClick(object sender, EventArgs e)
        {
            CodeField.SelectionStart = 0;
            CodeField.SelectionLength = CodeField.Text.Length;
        }

        private void PagesTabPressDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                int index = PagesTab.SelectedIndex;
                Pages[index].Close();
                Pages.Remove(Pages[index]);
                PagesTab.TabPages.Remove(PagesTab.TabPages[index]);
            }
        }

        private void RTBFontChanged(object sender, EventArgs e)
        {
            RowsNumbers.Font = CodeField.Font;
        }

        private void RTBScroll(object sender, EventArgs e)
        {
           UpdateRowsNumbers();
        }

        private void FormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                SaveButton.PerformClick();
            }
            if (e.KeyCode == Keys.Z && e.Control)
            {
                CancelButton.PerformClick();
            }
        }

        private void SetLocalisation(object sender, EventArgs e)
        {
            localisation.CurrentLocalisation = (sender as ToolStripMenuItem).Text;
            Locale();
        }

        private void TaskStatesAndTransClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\Automat.html");
        }

        private void TaskGraphAndTableClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Info\\Graph.html");
        }
    }
}
