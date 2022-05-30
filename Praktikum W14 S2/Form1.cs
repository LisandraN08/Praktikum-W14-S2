using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Praktikum_W14_S2
{
    public partial class FormTeam : Form
    {
        public FormTeam()
        {
            InitializeComponent();
        }
        MySqlConnection sqlConnect = new MySqlConnection("server=localhost;uid=root;pwd=;database=premier_league");
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        string sqlQuery;

        DataTable dtTeam = new DataTable();
        DataTable dtGoal = new DataTable();
        DataTable dtPelanggaran = new DataTable();
        DataTable dtMatch = new DataTable();
        int PosisiSekarang = 0;
        string topScore = "";
        string worstDiscipline = "";
        string teamID = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            sqlQuery = "SELECT t.team_id, t.team_name, m.manager_name, concat(t.home_stadium, ', ', t.city, '(',t.capacity,')') FROM team t, manager m, player p, dmatch d WHERE t.manager_id = m.manager_id and t.team_id = d.team_id GROUP BY 1;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeam);

            sqlQuery = "SELECT date_format(m.match_date, '%d/%m/%Y') as 'match_date', if(m.team_home='"+teamID+ "', 'HOME', 'AWAY') as 'Home/Away', concat('vs ',if (m.team_home = '" + teamID + "', m.team_away, m.team_home))as 'lawan', if (goal_home is null or goal_away is null, 'belum berlangsung', concat(m.goal_home, ' - ', m.goal_away))as 'score' FROM `match` m WHERE m.team_home = '" + teamID + "' or m.team_away = '" + teamID + "' ORDER BY m.match_date desc limit 5; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtMatch);
            dtGridViewMatch.DataSource = dtMatch;
            DataPemain(PosisiSekarang);
        }
        private void DataPemain(int posisi)
        {
            teamID = dtTeam.Rows[posisi][0].ToString();
            lblTeamName.Text = dtTeam.Rows[posisi][1].ToString();
            lblManager.Text = dtTeam.Rows[posisi][2].ToString();
            lblStadium.Text = dtTeam.Rows[posisi][3].ToString();
            PosisiSekarang = posisi;
            dtGoal = new DataTable();
            sqlQuery = "SELECT p.player_name, count(d.type), sum(if(d.type='GP', 1, 0)) FROM player p, dmatch d, team t WHERE p.player_id = d.player_id and(d.type = 'GO' or d.type = 'GP') and t.team_id=p.team_id and t.team_name='" + lblTeamName.Text + "' GROUP BY 1 ORDER BY 2 desc; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtGoal);
            topScore = dtGoal.Rows[0][0].ToString() + " " + dtGoal.Rows[0][1].ToString() + "(" + dtGoal.Rows[0][2].ToString() + ")";
            lblTopScorer.Text = topScore;
            dtPelanggaran = new DataTable();
            sqlQuery = "SELECT p.player_name, sum(if(d.type = 'CY',1,0)), sum(if(d.type = 'CR',1,0)), sum(if(d.type = 'CR',3,0)) + sum(if(d.type = 'CY',1,0)) FROM player p, dmatch d, team t WHERE p.player_id = d.player_id and(d.type = 'CY' or d.type = 'CR') and t.team_id=p.team_id and t.team_name='" + lblTeamName.Text + "' GROUP BY 1 ORDER BY 4 desc; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPelanggaran);
            worstDiscipline = dtPelanggaran.Rows[0][0].ToString() + ", " + dtPelanggaran.Rows[0][1].ToString() + " Yellow Card and " + dtPelanggaran.Rows[0][2].ToString() + " Red Card";
            lblWorstDiscipline.Text = worstDiscipline;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            DataPemain(0);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang > 0)
            {
                PosisiSekarang--;
                DataPemain(PosisiSekarang);

            }
            else
            {
                MessageBox.Show("Data Sudah Data Pertama");
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang < dtTeam.Rows.Count - 1)
            {
                PosisiSekarang++;
                DataPemain(PosisiSekarang);
            }
            else
            {
                MessageBox.Show("Data Sudah Data Terakhir");
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            DataPemain(dtTeam.Rows.Count - 1);
        }
    }
}
