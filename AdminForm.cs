﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace 学籍管理系统
{
    public partial class AdminForm : Form
    {
        MySqlConnectionStringBuilder connectStringBuilder;
        MySqlConnection myConnection;
        MySqlCommand myCommand;
        MySqlDataAdapter myAdapter;
        DataTable myDataTable;
        BindingSource professionBindingSource = new BindingSource();
        BindingSource classBindingSource = new BindingSource();
        BindingSource tableBindingSource = new BindingSource();
        public AdminForm()
        {
            InitializeComponent();
            #region 初始化数据库连接
            connectStringBuilder = new MySqlConnectionStringBuilder();
            connectStringBuilder.Database = "studentinfomanagedatabase";
            connectStringBuilder.Server = "localhost";
            connectStringBuilder.UserID = "root";
            connectStringBuilder.Password = "slowly";
            #endregion
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.账户信息表”中。您可以根据需要移动或删除它。
            this.账户信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.账户信息表);
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.学生信息表”中。您可以根据需要移动或删除它。
            this.学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.学生信息表”中。您可以根据需要移动或删除它。
            this.学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.学生信息表”中。您可以根据需要移动或删除它。
            this.学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.学生信息表”中。您可以根据需要移动或删除它。
            this.学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
            #region 学校管理panel
            // TODO:  这行代码将数据加载到表“studentinfomanagedatabaseDataSet.学校信息表”中。您可以根据需要移动或删除它。
            this.学校信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学校信息表);

            collegeCodeComboBox.SelectedValueChanged += collegeCodeComboBox_SelectedValueChanged;
            professionCodeComboBox.SelectedValueChanged += professionCodeComboBox_SelectedValueChanged;
            classCodeComboBox.SelectedValueChanged += classCodeComboBox_SelectedValueChanged;

            myConnection = new MySqlConnection(connectStringBuilder.ConnectionString);
            // 初始化专业信息栏
            string professionSelectQuery = "select 专业名, 专业代码 from 专业信息表 where (院系编号 = '$collegeCode')";
            professionSelectQuery = professionSelectQuery.Replace("$collegeCode", collegeCodeComboBox.Text);
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionComboBox, "专业名");
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionCodeComboBox, "专业代码");
            //professionCodeComboBox.DataBindings.Add("Text", professionBindingSource, "专业代码", true, DataSourceUpdateMode.OnPropertyChanged);
            // 初始化班级信息栏
            string classSelectQuery = "select 班级名称, 班级号 from 班级信息表 where (专业代码 = '$professionCode')";
            classSelectQuery = classSelectQuery.Replace("$professionCode", professionCodeComboBox.Text);
            UpdateDataSource(classBindingSource, classSelectQuery, classComboBox, "班级名称");
            UpdateDataSource(classBindingSource, classSelectQuery, classCodeComboBox, "班级号");
            //classCodeComboBox.DataBindings.Add("Text", classBindingSource, "班级号", true, DataSourceUpdateMode.OnPropertyChanged);
            // 初始化汇总信息表
            SetCollegeTableDataGridView();


            #endregion
        }
        //
        // 更新DataGridView选中状态
        //
        void classCodeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow myRow in collegeTableDataGridView.Rows)
            {
                if (myRow.Cells["班级号"].Value.ToString() == classCodeComboBox.Text)
                {
                    collegeTableDataGridView.ClearSelection();
                    myRow.Selected = true;
                    collegeTableDataGridView.CurrentCell = myRow.Cells["班级号"];
                }
            }
        }
        //
        // 更新班级信息栏
        //
        void professionCodeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string classSelectQuery = "select 班级名称, 班级号 from 班级信息表 where (专业代码 = '$professionCode')";
            classSelectQuery = classSelectQuery.Replace("$professionCode", professionCodeComboBox.Text);
            UpdateDataSource(classBindingSource, classSelectQuery, classComboBox, "班级名称");
            UpdateDataSource(classBindingSource, classSelectQuery, classCodeComboBox, "班级号");
            //MessageBox.Show("heh");
        }

        //
        // 更新专业信息栏
        //
        void collegeCodeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string professionSelectQuery = "select 专业名, 专业代码 from 专业信息表 where (院系编号 = '$collegeCode')";
            professionSelectQuery = professionSelectQuery.Replace("$collegeCode", collegeCodeComboBox.Text);
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionComboBox, "专业名");
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionCodeComboBox, "专业代码");
        }
        //
        // 用于更新数据源的方法
        //
        private void UpdateDataSource(BindingSource myBindingSource, string commandString, ComboBox updateComboBox, string DataMember)
        {
            myCommand = myConnection.CreateCommand();
            myConnection.Close();
            myCommand.CommandText = commandString;
            myAdapter = new MySqlDataAdapter(myCommand);
            //
            // 需要创建一个新的引用, 否则无法获取更新后的数据源, 目前不知道为什么
            // 使用ComboBox控件的DataBindings绑定数据源后也无法更新, 且只能获取数据表的一行记录, 目前不知道为什么
            //
            myDataTable = new DataTable();
            myAdapter.Fill(myDataTable);
            myDataTable.TableName = "InfoTable";
            myBindingSource.DataSource = myDataTable;
            //
            // 如果使用的是SelectedIndexChanged事件 则给DataSource传递引用需要在DisplayMember和ValueMember之后
            // 如果使用的是SelectedValueChanged事件 则给DataSource传递引用需要在DisplayMember和ValueMember之前
            //
            updateComboBox.DataSource = myBindingSource;
            updateComboBox.DisplayMember = DataMember;
            updateComboBox.ValueMember = DataMember;
            updateComboBox.FormattingEnabled = true;
        }
        private void SetCollegeTableDataGridView()
        {
            // 初始化汇总信息表
            string totalTableSelectQuery = "select 班级信息表.班级号, 班级名称, 专业信息表.专业代码, 专业名,  专业信息表.院系编号, 学院名, tmp.班级人数 " +
                                            "from 班级信息表 join 专业信息表 on 班级信息表.专业代码 = 专业信息表.专业代码 " +
                                            "join 学校信息表 on 专业信息表.院系编号 = 学校信息表.院系编号 " +
                                            "left join (SELECT 学生信息表.班级号, COUNT(学生信息表.班级号) AS '班级人数' " +
                                            "FROM 学生信息表  group by 班级号) as tmp on 班级信息表.班级号 = tmp.班级号";
            myCommand = myConnection.CreateCommand();
            myConnection.Close();
            myCommand.CommandText = totalTableSelectQuery;
            myAdapter = new MySqlDataAdapter(myCommand);
            myDataTable = new DataTable();
            myAdapter.Fill(myDataTable);
            myDataTable.TableName = "Table";
            tableBindingSource.DataSource = myDataTable;

            collegeTableDataGridView.DataSource = tableBindingSource;
            // 初始化选中行
            foreach (DataGridViewRow myRow in collegeTableDataGridView.Rows)
            {
                if (myRow.Cells["班级号"].Value.ToString() == classCodeComboBox.Text)
                {
                    collegeTableDataGridView.ClearSelection();
                    myRow.Selected = true;
                    collegeTableDataGridView.FirstDisplayedScrollingColumnIndex = myRow.Index;
                }
            }
        }
        //
        // 专业代码查询按钮
        //
        private void professionCodeQueryButton_Click(object sender, EventArgs e)
        {
            QueryAndSelect("专业代码", professionCodeComboBox);
        }
        //
        // 班级号查询按钮
        //
        private void classCodeQueryButton_Click(object sender, EventArgs e)
        {
            QueryAndSelect("班级号", classCodeComboBox);
        }
        //
        // 院系编号查询
        //
        private void collegeCodeQueryButton_Click(object sender, EventArgs e)
        {
            QueryAndSelect("院系编号", collegeCodeComboBox);
        }

        private void QueryAndSelect(string queryFieldString, ComboBox queryFieldComboBox)
        {
            bool firstTimeGetIn = true;
            collegeTableDataGridView.ClearSelection();
            foreach (DataGridViewRow myRow in collegeTableDataGridView.Rows)
            {
                if (myRow.Cells[queryFieldString].Value.ToString() == queryFieldComboBox.Text)
                {
                    myRow.Selected = true;
                    // 设置选中行为当前活动行
                    if (firstTimeGetIn)
                    {
                        collegeTableDataGridView.CurrentCell = myRow.Cells[queryFieldString];
                        firstTimeGetIn = false;
                    }
                }
            }
        }

        private void DeleteOneRecord(string deletedTableName, string primaryField, string value)
        {
            string deleteCommand = "delete from $deletedTableName where ( $primaryField = @value )";
            deleteCommand = deleteCommand.Replace("$deletedTableName", deletedTableName);
            deleteCommand = deleteCommand.Replace("$primaryField", primaryField);
            //MessageBox.Show(deleteCommand);
            myCommand = myConnection.CreateCommand();
            myCommand.CommandText = deleteCommand;

            myCommand.Parameters.AddWithValue("@value", value);
            myAdapter = new MySqlDataAdapter(myCommand);
            myAdapter.Fill(new DataTable());
        }

        private void AddOneRecord(MySqlCommand addCommand)
        {
            myAdapter = new MySqlDataAdapter(addCommand);
            myAdapter.Fill(new DataTable());
        }

        private void classInfoDeleteButton_Click(object sender, EventArgs e)
        {
            //    
            // 应该先进行检查外键约束, 当前删除项对应的学生信息表中相应项是否为空, 并给出提示
            //
            if (true)
            {

            }
            if (classCodeComboBox.Text != string.Empty)
            {
                DeleteOneRecord("班级信息表", "班级号", classCodeComboBox.Text);
                classBindingSource.RemoveCurrent();
                SetCollegeTableDataGridView();
            }

        }

        private void professionInfoDeleteButton_Click(object sender, EventArgs e)
        {
            //    
            // 应该先进行检查外键约束, 当前删除项对应的班级信息表中相应项是否为空, 并给出提示
            //
            if (true)
            {

            }
            if (professionCodeComboBox.Text != string.Empty)
            {
                DeleteOneRecord("专业信息表", "专业代码", professionCodeComboBox.Text);
                professionBindingSource.RemoveCurrent();
            }
        }

        private void collegeInfoDeleteButton_Click(object sender, EventArgs e)
        {
            //    
            // 应该先进行检查外键约束, 当前删除项对应的专业信息表中相应项是否为空, 并给出提示
            //
            if (true)
            {

            }
            if (collegeCodeComboBox.Text != string.Empty)
            {
                DeleteOneRecord("学校信息表", "院系编号", collegeCodeComboBox.Text);
                学校信息表BindingSource.RemoveCurrent();
            }

        }

        private void classInfoAddButton_Click(object sender, EventArgs e)
        {
            MySqlCommand addCommand = myConnection.CreateCommand();
            string addCommandString = "insert into 班级信息表 values(@classCode, @className, @professionCode, @studentCountOfClass)";
            addCommand.CommandText = addCommandString;
            addCommand.Parameters.AddWithValue("@classCode", classCodeComboBox.Text);
            addCommand.Parameters.AddWithValue("@className", classComboBox.Text);
            addCommand.Parameters.AddWithValue("@professionCode", professionCodeComboBox.Text);
            addCommand.Parameters.AddWithValue("@studentCountOfClass", 0);
            AddOneRecord(addCommand);
            SetCollegeTableDataGridView();

            string classSelectQuery = "select 班级名称, 班级号 from 班级信息表 where (专业代码 = '$professionCode')";
            classSelectQuery = classSelectQuery.Replace("$professionCode", professionCodeComboBox.Text);
            UpdateDataSource(classBindingSource, classSelectQuery, classComboBox, "班级名称");
            UpdateDataSource(classBindingSource, classSelectQuery, classCodeComboBox, "班级号");
        }

        private void professionInfoAddButton_Click(object sender, EventArgs e)
        {
            MySqlCommand addCommand = myConnection.CreateCommand();
            string addCommandString = "insert into 专业信息表 values(@professionCode, @professionName, @collegeCode, @classCountOfProfession)";
            addCommand.CommandText = addCommandString;
            addCommand.Parameters.AddWithValue("@professionCode", professionCodeComboBox.Text);
            addCommand.Parameters.AddWithValue("@professionName", professionComboBox.Text);
            addCommand.Parameters.AddWithValue("@collegeCode", collegeCodeComboBox.Text);
            addCommand.Parameters.AddWithValue("@classCountOfProfession", 0);
            AddOneRecord(addCommand);
            string professionSelectQuery = "select 专业名, 专业代码 from 专业信息表 where (院系编号 = '$collegeCode')";
            professionSelectQuery = professionSelectQuery.Replace("$collegeCode", collegeCodeComboBox.Text);
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionComboBox, "专业名");
            UpdateDataSource(professionBindingSource, professionSelectQuery, professionCodeComboBox, "专业代码");
        }

        private void collegeInfoAddButton_Click(object sender, EventArgs e)
        {
            MySqlCommand addCommand = myConnection.CreateCommand();
            string addCommandString = "insert into 学校信息表 values(@collegeCode, @collegeName, @professionCountOfCollege)";
            addCommand.CommandText = addCommandString;
            addCommand.Parameters.AddWithValue("@collegeCode", collegeCodeComboBox.Text);
            addCommand.Parameters.AddWithValue("@collegeName", collegeComboBox.Text);
            addCommand.Parameters.AddWithValue("@professionCountOfCollege", 0);
            MessageBox.Show(addCommand.CommandText);
            AddOneRecord(addCommand);

        }

        private void addStudentInfoButton_Click(object sender, EventArgs e)
        {
            MyException myError;
            try
            {
                this.Validate();
                this.学生信息表BindingSource.EndEdit();


                #region 定义一些约束检查
                //
                // 找到当前输入专业名的专业代码
                // 
                DataRowCollection professionTableRows = 专业信息表TableAdapter.GetDataByProfessionName(专业名TextBox.Text).Rows;
                if (professionTableRows.Count != 0)
                {
                    // 获取代码字符串
                    string professionCode;
                    professionCode = professionTableRows[0]["专业代码"].ToString();
                    // 找到当前专业代码的班级号集合
                    DataRowCollection classTableRows = 班级信息表TableAdapter.GetDataByProfessionCode(professionCode).Rows;
                    if (!classTableRows.Contains(班级号TextBox.Text))
                    {
                        myError = new MyException(班级号TextBox.Text + "在该专业不存在, 请重新输入");
                        throw myError;
                    }
                }
                else
                {
                    myError = new MyException(专业名TextBox.Text + "不存在, 请重新输入");
                    throw myError;
                }

                if (!(性别TextBox.Text.Equals("男") || 性别TextBox.Text.Equals("女")))
                {
                    myError = new MyException( "性别: " + 性别TextBox.Text + "不存在, 请重新输入");
                    throw myError;
                }
                #endregion

                学生信息表TableAdapter.Insert(学号TextBox.Text, 姓名TextBox.Text, 性别TextBox.Text, 出生日期DateTimePicker.Value,
                    政治面貌TextBox.Text, 入学时间DateTimePicker.Value, 院系名TextBox.Text, 专业名TextBox.Text, 班级号TextBox.Text,
                    电话号码TextBox.Text, 身份证号码TextBox.Text, 详细家庭住址TextBox.Text, null, 备注TextBox.Text);
                //
                // 直接向数据库中插入新数据后, dategridView不会更新, 需要fill一次更新bindSource
                //
                学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
                MessageBox.Show("Add Successful");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Add failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void queryByNameButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.FillByStudentName(this.studentinfomanagedatabaseDataSet.学生信息表, 姓名TextBox.Text);
        }

        private void queryByPoliticalStatusButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.FillByPoliticalStatus(this.studentinfomanagedatabaseDataSet.学生信息表, 政治面貌TextBox.Text);
        }

        private void queryByCollegeNameButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.FillByCollegeName(this.studentinfomanagedatabaseDataSet.学生信息表, 院系名TextBox.Text);
        }

        private void queryByProfessionNameButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.FillByProfessionName(this.studentinfomanagedatabaseDataSet.学生信息表, 专业名TextBox.Text);
        }

        private void queryByClassCodeButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.FillByClassCode(this.studentinfomanagedatabaseDataSet.学生信息表, 班级号TextBox.Text);
        }

        private void updateStudentInfoButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.学生信息表BindingSource.EndEdit();
                this.学生信息表TableAdapter.Update(this.studentinfomanagedatabaseDataSet.学生信息表);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Update failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteStudentInfoButton_Click(object sender, EventArgs e)
        {

            try
            {
                var reasult = MessageBox.Show("是否删除选中行数据", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reasult == DialogResult.Yes)
                {
                    int selectRowsCount;
                    selectRowsCount = dataGridView1.SelectedRows.Count;
                    DataGridViewRow[] myRows = new DataGridViewRow[selectRowsCount];
                    dataGridView1.SelectedRows.CopyTo(myRows, 0);
                    for (int i = 0; i < selectRowsCount; i++)
                    {
                        dataGridView1.Rows.Remove(myRows[i]);
                    }
                    学生信息表TableAdapter.Update(this.studentinfomanagedatabaseDataSet.学生信息表);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Delete failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cleanQueryButton_Click(object sender, EventArgs e)
        {
            学生信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.学生信息表);
        }

        private void choosePictureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDiaolg = new OpenFileDialog();
            string imageFilePath;
            fileDiaolg.Filter = "图片文件|*.jpg; *.jpeg; *.png; *.bmp|JPEG|*.jpg; *.jpeg|PNG|*.png|位图文件|*.bmp";
            if (fileDiaolg.ShowDialog() == DialogResult.OK)
            {
                imageFilePath = fileDiaolg.FileName;
                照片PictureBox.Image = Image.FromFile(imageFilePath);
            }
        }

        private void accountNameQueryButton_Click(object sender, EventArgs e)
        {
            账户信息表TableAdapter.FillByAccountName(this.studentinfomanagedatabaseDataSet.账户信息表, 账户名TextBox.Text);
        }

        private void accountCategoryButton_Click(object sender, EventArgs e)
        {
            账户信息表TableAdapter.FillByAccountCategory(this.studentinfomanagedatabaseDataSet.账户信息表, 账户类别TextBox.Text);
        }

        private void accountAuthorityButton_Click(object sender, EventArgs e)
        {
            账户信息表TableAdapter.FillByAccountAuthority(this.studentinfomanagedatabaseDataSet.账户信息表, int.Parse(账户权限TextBox.Text));
        }

        private void cleanAccountQueryButton_Click(object sender, EventArgs e)
        {
            账户信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.账户信息表);
        }

        private void accountInfoUpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.账户信息表BindingSource.EndEdit();
                this.账户信息表TableAdapter.Update(this.studentinfomanagedatabaseDataSet.账户信息表);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Update failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addAccountRecordbutton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.账户信息表BindingSource.EndEdit();

                账户信息表TableAdapter.Insert(账户名TextBox.Text, 账户密码TextBox.Text, 账户类别TextBox.Text, int.Parse(账户权限TextBox.Text));
                //
                // 直接向数据库中插入新数据后, dategridView不会更新, 需要fill一次更新bindSource
                //
                账户信息表TableAdapter.Fill(this.studentinfomanagedatabaseDataSet.账户信息表);
                MessageBox.Show("Add Successful");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Add failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void deleteAccountRecordButton_Click(object sender, EventArgs e)
        {
            try
            {
                var reasult = MessageBox.Show("是否删除选中行数据", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reasult == DialogResult.Yes)
                {
                    int selectRowsCount;
                    selectRowsCount = 账户信息表DataGridView.SelectedRows.Count;
                    DataGridViewRow[] myRows = new DataGridViewRow[selectRowsCount];
                    账户信息表DataGridView.SelectedRows.CopyTo(myRows, 0);
                    for (int i = 0; i < selectRowsCount; i++)
                    {
                        账户信息表DataGridView.Rows.Remove(myRows[i]);
                    }
                    账户信息表TableAdapter.Update(this.studentinfomanagedatabaseDataSet.账户信息表);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Delete failed:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitSystemLable_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    public class MyException : ApplicationException
    {
        public MyException(string message) : base(message) { }
    }
}
