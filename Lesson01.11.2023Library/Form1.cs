using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Xml;

namespace Lesson01._11._2023Library
{
    public partial class Form1 : Form
    {
        SQLiteConnection conn;
        DataTable dtAutor;
        DataTable dtBooks;
        SQLiteDataAdapter adAutor;
        SQLiteDataAdapter adBooks;
        public Form1()
        {
            InitializeComponent();

        }

        private void выбратьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                conn = new SQLiteConnection("DataSource=" + filename);
                conn.Open();
                string sqltext = "select name from sqlite_master where type='table'";
                using (SQLiteCommand cmd = new SQLiteCommand(sqltext, conn))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    CreateToolStripMenuItem.Enabled = !reader.HasRows;
                    if (reader.HasRows)
                    {
                        FillAutor();
                        FillBooks();
                    }
                    reader.Close();
                    conn.Close();
                }

            }
        }

        private void CreateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sqltext = "create table Autors (id int primary key, name varchar(20),descripton varchar(20));" +
                "create table Books (id int primary key, id_autor int, name varchar(20),descripton varchar(20));";
            using(SQLiteCommand cmd= new SQLiteCommand(sqltext,conn)){
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            CreateToolStripMenuItem.Enabled=false;

        }
        private void FillAutor()
        {
            string sqltextA = "select id,name,descripton from autors";
            adAutor = new SQLiteDataAdapter(sqltextA, conn);
            adAutor.SelectCommand = new SQLiteCommand(sqltextA, conn);
            SQLiteCommandBuilder cb = new SQLiteCommandBuilder(adAutor);
            dtAutor = new DataTable();
            adAutor.Fill(dtAutor);
            DGAutor.DataSource = dtAutor;
            dtAutor.Columns[0].AutoIncrement= true;
            dtAutor.Columns[0].AutoIncrementStep= 1;
            dtAutor.Columns[0].AutoIncrementSeed = 10;


        }
        private void FillBooks(string author_id="")
        {
            string sqltextB = "select books.id,books.id_autor," +
                " autors.name, books.name, books.descripton from books"+
                " join autors on autors.id=books.id_autor";
            if (!string.IsNullOrEmpty(author_id))
                sqltextB = sqltextB + " where id_autor=" + author_id;
            dtBooks = new DataTable();
            adBooks = new SQLiteDataAdapter(sqltextB, conn);
            adBooks.SelectCommand =new SQLiteCommand(sqltextB,conn);
            sqltextB = "insert into books values (@id,@id_autor,@name,@descripton);";
            adBooks.InsertCommand = new SQLiteCommand(sqltextB,conn);
            adBooks.InsertCommand.Parameters.Add("id", DbType.Int32, 5, "id");
            adBooks.InsertCommand.Parameters.Add("id_autor", DbType.Int32, 5, "id_autor");
            adBooks.InsertCommand.Parameters.Add("name", DbType.String, 20, "name");
            adBooks.InsertCommand.Parameters.Add("descripton", DbType.String, 20, "descripton");
            sqltextB = "update books set id_autor=@id_autor,name=@name,descripton=@descripton where id=@id";
            adBooks.UpdateCommand = new SQLiteCommand(sqltextB,conn);
            adBooks.UpdateCommand.Parameters.Add("id", DbType.Int32, 5, "id");
            adBooks.UpdateCommand.Parameters.Add("id_autor", DbType.Int32, 5, "id_autor");
            adBooks.UpdateCommand.Parameters.Add("name", DbType.String, 20, "name");
            adBooks.UpdateCommand.Parameters.Add("descripton", DbType.String, 20, "descripton");
            sqltextB = "delete from books where id=@id";
            adBooks.DeleteCommand= new SQLiteCommand(sqltextB,conn);
            adBooks.DeleteCommand.Parameters.Add("id", DbType.Int32, 5, "id");

            adBooks.Fill(dtBooks);
            DGBooks.DataSource = dtBooks;

            dtBooks.Columns[0].AutoIncrement = true;
            dtBooks.Columns[0].AutoIncrementStep = 1;
            dtBooks.Columns[0].AutoIncrementSeed = 10;
            adAutor.Fill(dataSet1);
            MessageBox.Show(DGBooks.Columns[1].Name);
            (DGBooks.Columns["AutorName"] as DataGridViewComboBoxColumn).DataSource = dtAutor;
            (DGBooks.Columns["AutorName"] as DataGridViewComboBoxColumn).DataPropertyName= "id_autor";
            (DGBooks.Columns["AutorName"] as DataGridViewComboBoxColumn).ValueMember= "ID";
            (DGBooks.Columns["AutorName"] as DataGridViewComboBoxColumn).DisplayMember= "Name";
        }

        private void сохранитьИзмененияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            adAutor.Update(dtAutor);
            adBooks.Update(dtBooks);
        }

        private void DGAutor_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            FillBooks(DGAutor[0, DGAutor.CurrentRow.Index].Value.ToString());

        }
    }
}
