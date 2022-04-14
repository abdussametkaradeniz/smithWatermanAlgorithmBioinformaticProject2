using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace smithWatermanAlgorithmBioinformaticProject2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            txtGap.Text = "-2";
            txtMatch.Text = "1";
            txtMissmatch.Text = "-1";
        }

       
        /// <summary>
        /// 
        /// 
        /// GLOBAL DEĞİŞKENLERİN OLDUĞU KISIM
        /// 
        /// 
        /// </summary>

        OpenFileDialog openfiledialog = new OpenFileDialog();
        public string sequence1TxtPath, sequence2TxtPath, txt1String, txt2String, combination1, combination2;
        public int txt1Count, txt2Count, match, missmatch, gap, diagonal, score,maxCellValue = 0,maxCellValueRowIndex,maxCellValueCellIndex;
        public int[] valuesLastState = new int[3];
        

        /*************************************************************************************************************************/

        /// <summary>
        /// 
        /// 
        /// BURADAN AŞAĞISINDA BUTONLARIN CLİCK EVENTLERİ MEVCUT
        /// 
        /// 
        /// </summary>


        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSequence1Browse_Click(object sender, EventArgs e)
        {
            SelectFile();
            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                txt1path.Text = openfiledialog.FileName;
                sequence1TxtPath = openfiledialog.FileName.ToString();
            }
        }

        private void btnSequence2Browse_Click(object sender, EventArgs e)
        {
            SelectFile();
            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                txt2path.Text = openfiledialog.FileName;
                sequence2TxtPath = openfiledialog.FileName.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BtnDoldurDriverMetod();
        }

    

      

        /**************************************************************************************************************************/

        /// <summary>
        /// 
        /// BURADAN AŞAĞISINDA ELLE YAZILAN FONKSİYONLAR BULUNMAKTADIR
        /// 
        /// </summary>


        //dosya seçmek için gereken metot 
        public void SelectFile()
        {
            openfiledialog.RestoreDirectory = true;
            openfiledialog.Title = "Browse Txt Sequence";
            openfiledialog.DefaultExt = "txt";
            openfiledialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openfiledialog.CheckFileExists = true;
            openfiledialog.CheckPathExists = true;
        }


          
        

        public void Read()
        {
            using (StreamReader file = new StreamReader(sequence1TxtPath))
            {
                int counter = 0;
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0)
                    {
                        txt1Count = int.Parse(line);
                    }
                    else
                    {
                        txt1String = line;
                    }
                    counter++;
                }
                file.Close();
            }
            using (StreamReader file = new StreamReader(sequence2TxtPath))
            {
                int counter = 0;
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0)
                    {
                        txt2Count = int.Parse(line);
                    }
                    else
                    {
                        txt2String = line;
                    }
                    counter++;
                }
                file.Close();
            }
        }

        //satırları nükleotitler ile dolduran metot
        public void fillGridViewColumn()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnCount = txt1Count + 2;
            string[] txt1Karakterleri = new string[txt1String.Length];
            for (int i = 0; i < txt1String.Length; i++)
            {
                txt1Karakterleri[i] = txt1String[i].ToString();
            }
            for (int i = 0; i < txt1Karakterleri.Length; i++)
            {
                dataGridView1.Columns[i + 1].Name = txt1Karakterleri[i].ToString();
                dataGridView1.Columns[i + 1].HeaderText = txt1Karakterleri[i].ToString();
                dataGridView1.Columns[i + 1].DataPropertyName = txt1Karakterleri[i].ToString();
            }

        }

      


        //sütunları nükleotitler ile dolduran metot
        public void fillGridViewRow()
        {            
            string[] txt2Karakterleri = new string[txt2String.Length];
            for (int i = 0; i < txt2String.Length; i++)
            {
                txt2Karakterleri[i] = txt2String[i].ToString();
            }
            for (int i = 0; i < txt2Karakterleri.Length + 1; i++)
            {
                if (i == 0)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[0].Cells[0].Value = 0;
                }
                else
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].HeaderCell.Value = txt2Karakterleri[i - 1].ToString();
                }
            }
        }

        //satır ve sütunları dolduran fonksiyonları çağıran fonksiyon
        public void BtnDoldurDriverMetod()
        {
            combination1 = "";
            combination2 = "";
            Stopwatch calculate = new Stopwatch();
            dataGridView1.Columns.Clear();
            match = int.Parse(txtMatch.Text);
            missmatch = int.Parse(txtMissmatch.Text);
            gap = int.Parse(txtGap.Text);
            Read();
            calculate.Start();         
            fillGridViewColumn();
            fillGridViewRow();            
            fillGap();          
            fillCells();
            traceback(maxCellValueRowIndex,maxCellValueCellIndex);           
            calculate.Stop();
            TimeSpan runtime = calculate.Elapsed;
            string Sonuc = String.Format("{0:00}:{1:00}:{2:00}.{3:00}.{4:00}", runtime.Hours, runtime.Minutes, runtime.Seconds, runtime.Milliseconds,runtime.Ticks);
            txtCalismaSuresi.Text = Sonuc;
            writeTxt();
        }

        //gap değerine göre satır ve sütunları dolduran fonksiyon
        public void fillGap()
        {
            for (int i = 1; i < txt1Count + 1; i++)
            {             
                int value = gap + int.Parse((dataGridView1.Rows[0].Cells[i - 1].Value).ToString());
                dataGridView1.Rows[0].Cells[i].Value = value < 0 ? "0" : (gap + int.Parse((dataGridView1.Rows[0].Cells[i - 1].Value).ToString())).ToString();

            }
            for (int i = 1; i < txt2Count + 1; i++)
            {
                int value = gap + int.Parse((dataGridView1.Rows[i - 1].Cells[0].Value).ToString());
                dataGridView1.Rows[i].Cells[0].Value = value < 0 ? "0" : (gap + int.Parse((dataGridView1.Rows[i - 1].Cells[0].Value).ToString())).ToString();
            }
        }

       //MATCH DURUMUNA BAKAN FONKSİYON
        public Boolean stateMatch(int CellI,int RowI)
        {
            bool value = dataGridView1.Rows[RowI].HeaderCell.Value.ToString().Equals(dataGridView1.Columns[CellI].HeaderText.ToString()) ? true : false;
            return value;
        }

        /// <summary>
        /// adım 1:
        ///     eşleşme varsa:
        ///         çaprazdaki değer + match değeri > 0 ise o değeri o sütuna yazıyoruz, 0 < ise 0 yazıyoruz
        ///     eşleşme yoksa:
        ///         soldaki değer + gap, yukarıdaki değer + gap, çaprazdaki değer + missmatch değerleri arasında,
        ///         0 dan büyük olan en büyük varsa oraya yazılır.
        ///         sıfırdan küçükse hepsi bu durumda oraya da 0 eklenir
        /// </summary>

       
        //hücreleri dolduran metot
        public void fillCells()
        {
            for (int rowI = 1; rowI <= txt2Count; rowI++)
            {
                for (int cellI = 1; cellI <= txt1Count; cellI++)
                {
                    if (stateMatch(cellI,rowI))
                    {
                        dataGridView1.Rows[rowI].Cells[cellI].Value = Diagonal(rowI, cellI);
                    }
                    else
                    {
                        int state = lastState(rowI,cellI);
                        if (state>0)
                        {
                            dataGridView1.Rows[rowI].Cells[cellI].Value = state.ToString();
                        }
                        else
                        {
                            dataGridView1.Rows[rowI].Cells[cellI].Value = "0";
                        }
                    }
                    if (int.Parse(dataGridView1.Rows[rowI].Cells[cellI].Value.ToString())>maxCellValue)
                    {
                        maxCellValue = int.Parse(dataGridView1.Rows[rowI].Cells[cellI].Value.ToString());
                        maxCellValueCellIndex = cellI;
                        maxCellValueRowIndex = rowI;
                    }
                }             
            }            
        }

        //soldan gelen degeri hesaplayan metot
        public int fromLeft(int rowIndex, int cellIndex)
        {
            return (int.Parse(dataGridView1.Rows[rowIndex].Cells[cellIndex - 1].Value.ToString()));
        }

        //yukarıdan gelen degeri hesaplayan metot
        public int fromUp(int rowIndex, int cellIndex)
        {
            return (int.Parse(dataGridView1.Rows[rowIndex - 1].Cells[cellIndex].Value.ToString()));
        }

        //çaprazdan gelen değeri hesaplayan metot
        public int Diagonal(int rowIndex, int cellIndex)
        {
            diagonal = returnDiagonalValue(rowIndex, cellIndex);
            if (dataGridView1.Rows[rowIndex].HeaderCell.Value.ToString().Equals(dataGridView1.Columns[cellIndex].HeaderText.ToString()))
            {
                diagonal = diagonal + match;
            }
            else
            {
                diagonal = diagonal + missmatch;
            }
            return diagonal;
        }

        //diagonali döndüren fonksiyon
        public int returnDiagonalValue(int rowIndex, int cellIndex)
        {
            return int.Parse(dataGridView1.Rows[rowIndex - 1].Cells[cellIndex - 1].Value.ToString());
        }

        //son durumu belirleyen metot
        public int lastState(int rowIndex, int cellIndex)
        {
            valuesLastState[0] = fromLeft(rowIndex, cellIndex) + gap;
            valuesLastState[1] = fromUp(rowIndex, cellIndex) + gap;
            valuesLastState[2] = Diagonal(rowIndex, cellIndex);
            int maxValue = returnMaxValue(valuesLastState);
            return maxValue;
        }

        //valueslaststate dizisindeki 3 değeri sürekli tekrar doldurup geri döndüren metot
        public int[] returnValuesLastStateArray(int rowIndex, int cellIndex)
        {
            valuesLastState[0] = fromLeft(rowIndex, cellIndex);
            valuesLastState[1] = fromUp(rowIndex, cellIndex);
            valuesLastState[2] = returnDiagonalValue(rowIndex, cellIndex);
            return valuesLastState;
        }

        //max değeri dönen fonksiyon
        public int returnMaxValue(int[] valuesLastState)
        {
            return valuesLastState.Max();
        }
    
        public int returnMinValue(int[] valuesLastState)
        {
            return valuesLastState.Min();
        }
        
        ///<summary>
        ///
        /// BU KISIMDAN AŞAĞISI TRACEBACK ADIMLARINI İÇERİR.
        /// 
        /// (maxCellValueRowIndex,maxCellValueCellIndex) noktasından başlayarak 0 a ulaşana kadar en büyük değere gideceğiz.
        /// 
        /// </summary>
        
        public void traceback(int maxCellValueRowIndex,int maxCellValueCellIndex)
        {
            while (true)
            {              
                if ((dataGridView1.Rows[maxCellValueRowIndex].HeaderCell.Value.ToString()).Equals(dataGridView1.Columns[maxCellValueCellIndex].HeaderCell.Value.ToString()))
                {
                    combination1 = combination1 + dataGridView1.Rows[maxCellValueRowIndex].HeaderCell.Value.ToString();
                    combination2 = combination2 + dataGridView1.Columns[maxCellValueCellIndex].HeaderCell.Value.ToString();
                    dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Style.BackColor = Color.Red;
                    maxCellValueRowIndex--;
                    maxCellValueCellIndex--;
                }
                else
                {
                    valuesLastState = returnValuesLastStateArray(maxCellValueRowIndex, maxCellValueCellIndex);
                    int maxValue = returnMaxValue(valuesLastState);
                    if (maxValue == fromLeft(maxCellValueRowIndex, maxCellValueCellIndex))
                    {
                        //sola ilerle
                        combination1 = combination1 + "--";
                        combination2 = combination2 + dataGridView1.Columns[maxCellValueCellIndex].HeaderText.ToString();
                        dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Style.BackColor = Color.Red;                        
                        maxCellValueCellIndex = maxCellValueCellIndex - 1;
                    }
                    else if(maxValue == fromUp(maxCellValueRowIndex, maxCellValueCellIndex)){
                        //yukarı ilerle
                        combination1 = combination1 + "--";
                        combination2 = combination2 + dataGridView1.Columns[maxCellValueCellIndex].HeaderText.ToString();
                        dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Style.BackColor = Color.Red;
                        maxCellValueRowIndex = maxCellValueRowIndex - 1;                       
                    }
                    else
                    {
                        //çapraz ilerle
                        combination1 = combination1 + dataGridView1.Rows[maxCellValueRowIndex].HeaderCell.Value.ToString();
                        combination2 = combination2 + dataGridView1.Columns[maxCellValueCellIndex].HeaderCell.Value.ToString();
                        dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Style.BackColor = Color.Red;
                        maxCellValueRowIndex--;
                        maxCellValueCellIndex--;
                    }

                }
                if (int.Parse(dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Value.ToString()) == 0)
                {
                    dataGridView1.Rows[maxCellValueRowIndex].Cells[maxCellValueCellIndex].Style.BackColor = Color.Red;
                    break;
                }
            }
        }



        public string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }

        public void writeTxt()
        {          
            txtRichCombination.Text = "Result " + "\n"
                + Reverse(combination1) + "\n"
                + Reverse(combination2) + "\n"
                + "score = " + maxCellValue*match + "\n"
                + "seq 1 " + txt1String + "\n"
                + "seq 2 " + txt2String;
        }




    }
}
