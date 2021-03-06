﻿using FinalProject.model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class Form1 : Form
    {

        DataTable teams;
        DataTable players;
        DataTable games;
        DataTable ranking;

        public Form1()
        {
            InitializeComponent();

        }

        /**
         *  TO IMPLEMENT
         *  1. Add, update, remove, players/teams
         *      - TextBox: fname, lname, 
         *      - number picker(with typing): number, age, height, weight
         *      - Combo Box: team (populate with current teams, so user cant enter invalid team.
         *  2. Arrows between trade tab to move players from one team to the other.
         *      then a trade button, which will have a confirmation box pop up.
         *      if accepted, run sql query to update traded players teams. if declined
         *      put players back to original teams?
         *  3. Schedule Tab?
         *      have a schedule table with games booked between teams. have ability to 
         *      update scores for past games. based on scores award winning teams points.
         *  4. Standings Tab?
         *      if implementing schedule and points, have standings?
         *      create a view, query teams tab, order by points? (should be easy)
         *  5. ANYTHING ELSE???
         * 
         **/





        /**
         * Loads all fields with apprpriate info
         **/
        private void Form1_Load(object sender, EventArgs e)
        {


            // TODO: This line of code loads data into the 'cprojectDataSet.players' table. You can move, or remove it, as needed.

            clearPlayerEditFields();
            clearPlayerViewFields();
            clearTeamEditFields();
            clearTeamViewFields();

            gbViewTeam.Visible = true;
            gbEditTeam.Visible = false;
            gbViewPlayer.Visible = true;
            gbEditPlayer.Visible = false;

            SoccerPlayerDB playersDB = new SoccerPlayerDB();
            SoccerTeamDB teamsDB = new SoccerTeamDB();
            SoccerScheduleDB gamesDB = new SoccerScheduleDB();
            try
            {

                showAllPlayers();

                showAllTeams();

                showAllGames();

                showRanking();
                /////// standings tab /////////

                // queries players table and updates dataGridViewPlayers
                

            }
            catch (SqlException sql)
            {
                MessageBox.Show("issue connection to the db " + sql.Errors);
            }
            catch (Exception ex)
            {
                MessageBox.Show("some other error " + ex.Message);
            }

        }

        /**
         * loads teams into their respective list boxes based on the team selected for the trade
         * 
         * TODO: check that the same team isnt selected in both boxes
         **/
        private void btn_loadTrade_Click(object sender, EventArgs e)
        {

            // clear lists before loading new elements
            lbTeam1.Items.Clear();
            lbTeam2.Items.Clear();

            try
            {
                SoccerPlayerDB playersDB = new SoccerPlayerDB();

                //populates players from the team selected from the combo box
                String t1 = cbTeam1.Text;
                String t2 = cbTeam2.Text;

                if (t1 != t2)
                {
                    //populates lb1 with players from the selected team in cb2
                    DataTable team1 = playersDB.getPlayersFromTeam(t1);

                    foreach (DataRow row in team1.Rows)
                    {
                        lbTeam1.Items.Add(row["fname"].ToString() + " " + row["lname"].ToString());

                        //lbTeam1.Items.AddRange(new object[] { row["id"].ToString(), row["fname"].ToString() + " " + row["lname"].ToString() });
                    }

                    //populates listbox 2 with the players form the selected team in cb2
                    DataTable team2 = playersDB.getPlayersFromTeam(t2);

                    foreach (DataRow row in team2.Rows)
                    {
                        lbTeam2.Items.Add(row["fname"].ToString() + " " + row["lname"].ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Please select different teams");
                }

            }
            catch (SqlException sql)
            {
                MessageBox.Show("issue connection to the db " + sql.Errors);
            }
            catch (Exception ex)
            {
                MessageBox.Show("some other error " + ex.Message);
            }

        }

        // Filter player
        private void btn_filter_Click(object sender, EventArgs e)
        {
            DataView view = new DataView(players);

            string filter = "";

            if (!string.IsNullOrWhiteSpace(txtViewPlayerFirstName.Text))
            {
                filter += string.Format("fname = '{0}' AND ", txtViewPlayerFirstName.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerLastName.Text))
            {
                filter += string.Format("lname = '{0}' AND ", txtViewPlayerLastName.Text);
            }

            if (cbViewPlayerTeam.Text != "")
            {
                filter += string.Format("team = '{0}' AND ", cbViewPlayerTeam.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerPosition.Text))
            {
                filter += string.Format("position = '{0}' AND ", txtViewPlayerPosition.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerAgeFrom.Text))
            {
                filter += string.Format("age >= '{0}' AND ", txtViewPlayerAgeFrom.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerAgeTo.Text))
            {
                filter += string.Format("age <= '{0}' AND ", txtViewPlayerAgeTo.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerHeightFrom.Text))
            {
                filter += string.Format("height >= '{0}' AND ", txtViewPlayerHeightFrom.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerHeightTo.Text))
            {
                filter += string.Format("height <= '{0}' AND ", txtViewPlayerHeightTo.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerWeightFrom.Text))
            {
                filter += string.Format("weight >= '{0}' AND ", txtViewPlayerWeightFrom.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerWeightTo.Text))
            {
                filter += string.Format("weight <= '{0}' AND ", txtViewPlayerWeightTo.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerNumberFrom.Text))
            {
                filter += string.Format("number >= '{0}' AND ", txtViewPlayerNumberFrom.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtViewPlayerNumberTo.Text))
            {
                filter += string.Format("number <= '{0}' AND ", txtViewPlayerNumberTo.Text);
            }

            if (filter != "")
            {
                filter = filter.Substring(0, filter.Length - 4); // Remove last AND

                // Set filter
                view.RowFilter = filter;

                //Update DataGridView
                dataGridViewPlayers.DataSource = view;

            }
            else // All fields are blank! Show all
            {
                showAllPlayers();
            }

        }

        private void showAllPlayers()
        {
            try
            {
                SoccerPlayerDB playersDB = new SoccerPlayerDB();
                players = playersDB.getAll();
                // queries players table and updates dataGridViewPlayers 
                dataGridViewPlayers.DataSource = players;

            }
            catch (SqlException sql)
            {
                MessageBox.Show("issue connection to the db " + sql.Errors);
            }
            catch (Exception ex)
            {
                MessageBox.Show("some other error " + ex.Message);
            }
        }

        private void showAllTeams()
        {
            SoccerTeamDB teamsDB = new SoccerTeamDB();
            teams = teamsDB.getAll();

            // Update Grid Views
            dataGridViewTeams.DataSource = teams;

            // Update ComboBox
            cbTeam1.Items.Clear();
            cbTeam2.Items.Clear();
            cbPlayerTeams.Items.Clear();
            cbViewPlayerTeam.Items.Clear();
            foreach (DataRow row in teams.Rows)
            {
                cbTeam1.Items.Add(row["tname"].ToString());
                cbTeam2.Items.Add(row["tname"].ToString());
                cbPlayerTeams.Items.Add(row["tname"].ToString());
                cbViewPlayerTeam.Items.Add(row["tname"].ToString());
            }
        }

        private void btnAddTeam_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerTeamDB db = new SoccerTeamDB();

            // Get team details
            String name = txtName.Text;

            // Verify if team already exist
            if (db.getById(name) == null)
            {
                String division = txtDivision.Text;
                SoccerTeam team = new SoccerTeam(name, division);

                try
                {
                    int result = db.addTeam(team); // Save team

                    if (result > 0)
                    {
                        MessageBox.Show(name + " was added successfully!", "Add Team", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearTeamEditFields();
                        showAllTeams();
                    }
                    else
                    {
                        MessageBox.Show(name + " cannot be added!", "Add Team Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else // If team already exist
            {
                MessageBox.Show(name + " already exist in database!", "Add Team Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearTeam_Click(object sender, EventArgs e)
        {
            clearTeamEditFields();
        }

        // Clear all Team fields (Edit options)
        private void clearTeamEditFields()
        {
            txtName.Text = "";
            txtDivision.Text = "";
            lblTeamId.Text = "";
            cbDeleteTeam.Checked = false;
        }

        // Clear all Team fields (View options)
        private void clearTeamViewFields()
        {
            txtFilterTeamName.Text = "";
            txtFilterDivision.Text = "";
            txtRankingFrom.Text = "";
            txtRankingTo.Text = "";
        }

        // Clear all `Player fields (Edit options)
        private void clearPlayerEditFields()
        {
            txtFName.Text = "";
            txtLName.Text = "";
            numNumber.Text = "";
            numAge.Text = "";
            txtPosition.Text = "";
            numHeight.Text = "";
            numWeight.Text = "";
            cbPlayerTeams.SelectedIndex = -1;
            lblPlayerId.Text = "";
            cbDeletePlayer.Checked = false;
        }

        // Clear all `Player fields (View options)
        private void clearPlayerViewFields()
        {
            txtViewPlayerFirstName.Text = "";
            txtViewPlayerLastName.Text = "";
            cbViewPlayerTeam.SelectedIndex = -1;
            txtViewPlayerPosition.Text = "";
            txtViewPlayerAgeFrom.Text = "";
            txtViewPlayerAgeTo.Text = "";
            txtViewPlayerHeightFrom.Text = "";
            txtViewPlayerHeightTo.Text = "";
            txtViewPlayerWeightFrom.Text = "";
            txtViewPlayerWeightTo.Text = "";
            txtViewPlayerNumberFrom.Text = "";
            txtViewPlayerNumberTo.Text = "";
        }

        private void btnUpdateTeam_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerTeamDB db = new SoccerTeamDB();

            // Get team details
            string name = txtName.Text;
            string id = lblTeamId.Text;

            // Verify if team exist
            if (db.getById(name) != null)
            {
                string division = txtDivision.Text;
                SoccerTeam team = new SoccerTeam(name, division);

                if (!cbDeleteTeam.Checked) // If user want to update the team
                {
                    try
                    {
                        int result = db.updateTeam(team, id); // Save team

                        if (result > 0)
                        {
                            MessageBox.Show(name + " was successfully updated!", "Update Team", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearTeamEditFields();
                            showAllTeams();
                        }
                        else
                        {
                            MessageBox.Show(name + " cannot be updated!", "Update Team Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else // User wants to delete the team
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete " + name, "Delete Team", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.No) // Do not delete!
                    {
                        cbDeleteTeam.Checked = false;
                    }
                    else
                    {
                        try
                        {
                            int result = db.deleteTeam(team); // Save team

                            if (result > 0)
                            {
                                MessageBox.Show(name + " successfully deleted!", "Delete Team", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                cbDeleteTeam.Checked = false;
                                clearTeamEditFields();
                                showAllTeams();
                            }
                            else
                            {
                                MessageBox.Show(name + " cannot be deleted!", "Delete Team Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }

            }
            else // If team doesn't exist
            {
                MessageBox.Show("Please, select team from Grid View", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dataGridViewTeams_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the index of the cell
            int rowIndex = e.RowIndex;

            // If user do not click on header
            if (rowIndex != -1)
            {
                DataGridViewRow row = dataGridViewTeams.Rows[rowIndex];

                // Update text box
                txtName.Text = row.Cells[0].Value.ToString();
                txtDivision.Text = row.Cells[2].Value.ToString();

                // Save ID
                lblTeamId.Text = row.Cells[0].Value.ToString();
            }
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorMsg = "Team name is required";
                errorTeamName.SetError(txtName, errorMsg);
                e.Cancel = true;
            }
            else // Remove error provider
            {
                errorTeamName.Clear();
            }
        }

        private void dataGridViewPlayers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the index of the cell
            int rowIndex = e.RowIndex;

            // If user do not click on header
            if (rowIndex != -1)
            {
                DataGridViewRow row = dataGridViewPlayers.Rows[rowIndex];

                // Update text box
                lblPlayerId.Text = row.Cells[0].Value.ToString();
                txtFName.Text = row.Cells[1].Value.ToString();
                txtLName.Text = row.Cells[2].Value.ToString();
                numNumber.Text = row.Cells[3].Value.ToString();
                numAge.Text = row.Cells[4].Value.ToString();
                txtPosition.Text = row.Cells[5].Value.ToString();
                numHeight.Text = row.Cells[6].Value.ToString();
                numWeight.Text = row.Cells[7].Value.ToString();
                cbPlayerTeams.Text = row.Cells[8].Value.ToString();
            }
        }

        private void btnAddPlayer_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerPlayerDB db = new SoccerPlayerDB();

            // Create player
            SoccerPlayer player = new SoccerPlayer();

            player.FirstName = txtFName.Text;
            player.LastName = txtLName.Text;
            player.Number = Convert.ToInt32(numNumber.Text);
            player.Age = Convert.ToInt32(numAge.Text);
            player.Position = txtPosition.Text;
            player.Height = Convert.ToInt32(numHeight.Text);
            player.Weight = Convert.ToInt32(numWeight.Text);
            player.Team = cbPlayerTeams.Text;

            // Verify if team already exist
            if (db.getPlayerId(player) == 0)
            {
                try
                {
                    int result = db.addPlayer(player); // Save player

                    if (result > 0)
                    {
                        MessageBox.Show(player.FirstName + " " + player.LastName + " was added successfully!", "Add Player", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearPlayerEditFields();
                        showAllPlayers();
                    }
                    else
                    {
                        MessageBox.Show(player.FirstName + " " + player.LastName + " cannot be added!", "Add Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else // If team player exist
            {
                MessageBox.Show(player.FirstName + " " + player.LastName + " already exist in database!", "Add Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearPlayer_Click(object sender, EventArgs e)
        {
            clearPlayerEditFields();
        }

        private void btnUpdatePlayer_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerPlayerDB db = new SoccerPlayerDB();
            SoccerPlayer player = new SoccerPlayer();

            // Verify if player exist
            if (lblPlayerId.Text != "")
            {
                // Create player
                player.FirstName = txtFName.Text;
                player.LastName = txtLName.Text;
                player.Number = Convert.ToInt32(numNumber.Text);
                player.Age = Convert.ToInt32(numAge.Text);
                player.Position = txtPosition.Text;
                player.Height = Convert.ToInt32(numHeight.Text);
                player.Weight = Convert.ToInt32(numWeight.Text);
                player.Team = cbPlayerTeams.Text;

                player.Id = Convert.ToInt32(lblPlayerId.Text);

                if (!cbDeletePlayer.Checked) // If user want to update the Player
                {
                    try
                    {
                        int result = db.updatePlayer(player); // Save team

                        if (result > 0)
                        {
                            MessageBox.Show(player.FirstName + " " + player.LastName + "  was successfully updated!", "Update Player", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clearPlayerEditFields();
                            showAllPlayers();
                        }
                        else
                        {
                            MessageBox.Show(player.FirstName + " " + player.LastName + " cannot be updated!", "Update Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else // User wants to delete the team
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete " + player.FirstName + " " + player.LastName, "Delete Player", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.No) // Do not delete!
                    {
                        cbDeletePlayer.Checked = false;
                    }
                    else
                    {
                        try
                        {
                            int result = db.deletePlayer(player); // Save team

                            if (result > 0)
                            {
                                MessageBox.Show(player.FirstName + " " + player.LastName + " was successfully deleted!", "Delete Player", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                cbDeletePlayer.Checked = false;
                                clearPlayerEditFields();
                                showAllPlayers();
                            }
                            else
                            {
                                MessageBox.Show(player.FirstName + " " + player.LastName + " cannot be deleted!", "Delete Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("Please, select player from Grid View", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Teams group control. Enable View
        private void rbViewTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (rbViewTeam.Checked)
            {
                gbViewTeam.Visible = true;
            }
            else
            {
                gbViewTeam.Visible = false;
            }
        }

        // Teams group control. Enable Edit
        private void rbEditTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEditTeam.Checked)
            {
                gbEditTeam.Visible = true;
            }
            else
            {
                gbEditTeam.Visible = false;
            }
        }

        // Clear fields en View Teams
        private void btnClearTeamFilter_Click(object sender, EventArgs e)
        {
            clearTeamViewFields();
            showAllTeams();
            errorFilterRankingTo.Clear();
            errorFilterRankingFrom.Clear();
        }

        // Filter teams in grid View.
        private void btnFilterTeam_Click(object sender, EventArgs e)
        {
            DataView view = new DataView(teams);

            string filter = "";

            if (!string.IsNullOrWhiteSpace(txtFilterTeamName.Text))
            {
                filter += string.Format("tname = '{0}' AND ", txtFilterTeamName.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtFilterDivision.Text))
            {
                filter += string.Format("division = '{0}' AND ", txtFilterDivision.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtRankingFrom.Text))
            {
                filter += string.Format("ranking >= '{0}' AND ", txtRankingFrom.Text);
            }

            if (!string.IsNullOrWhiteSpace(txtRankingTo.Text))
            {
                filter += string.Format("ranking <= '{0}' AND ", txtRankingTo.Text);
            }

            if (filter != "")
            {
                filter = filter.Substring(0, filter.Length - 4); // Remove last AND

                // Set filter
                view.RowFilter = filter;

                //Update DataGridView
                dataGridViewTeams.DataSource = view;

            }
            else // All fields are blank! Show all
            {
                showAllTeams();
            }
        }

        private void txtRankingTo_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int rankFrom;
            int rankTo;

            // If rank to is not empty
            if (txtRankingTo.Text != "")
            {

                // Try to convert into numbers              
                if (!int.TryParse(txtRankingTo.Text, out rankTo)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterRankingTo.SetError(txtRankingTo, errorMsg);
                    e.Cancel = true;
                }
                else
                {


                    if (txtRankingFrom.Text != "") // Rank to is greater than rank from
                    {
                        rankFrom = Convert.ToInt32(txtRankingFrom.Text);

                        if (rankTo < rankFrom)
                        {
                            errorMsg = "Ranking to cannot be greater than ranking to";
                            errorFilterRankingTo.SetError(txtRankingTo, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterRankingTo.Clear();
                        }
                    }
                    else // Remove error provider
                    {
                        errorFilterRankingTo.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterRankingTo.Clear();
            }
        }

        private void txtRankingFrom_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int rankFrom;
            int rankTo;

            // If rank to is not empty
            if (txtRankingFrom.Text != "")
            {
                // Try to convert into numbers
                if (!int.TryParse(txtRankingFrom.Text, out rankFrom)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterRankingFrom.SetError(txtRankingFrom, errorMsg);
                    e.Cancel = true;
                }
                else
                {
                    if (txtRankingTo.Text != "") // Rank to is greater than rank from
                    {
                        rankTo = Convert.ToInt32(txtRankingTo.Text);

                        if (rankTo < rankFrom)
                        {
                            errorMsg = "Ranking from cannot be lower than ranking to";
                            errorFilterRankingFrom.SetError(txtRankingFrom, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterRankingFrom.Clear();
                        }

                    }
                    else // Remove error provider
                    {
                        errorFilterRankingFrom.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterRankingFrom.Clear();
            }
        }

        // Player group control. Enable View
        private void rbViewPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbViewPlayer.Checked)
            {
                gbViewPlayer.Visible = true;
            }
            else
            {
                gbViewPlayer.Visible = false;
            }
        }

        // Player group control. Enable Edit
        private void rbEditPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEditPlayer.Checked)
            {
                gbEditPlayer.Visible = true;
            }
            else
            {
                gbEditPlayer.Visible = false;
            }
        }

        private void btnClearPlayerFilter_Click(object sender, EventArgs e)
        {

            errorFilterAgeFrom.Clear();
            errorFilterAgeTo.Clear();
            errorFilterHeightFrom.Clear();
            errorFilterHeightTo.Clear();
            errorFilterNumberFrom.Clear();
            errorFilterNumberTo.Clear();
            errorFilterWeightFrom.Clear();
            errorFilterWeightTo.Clear();
            clearPlayerViewFields();
            showAllPlayers();

        }

        private void txtViewPlayerAgeFrom_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int ageFrom;
            int ageTo;

            // If age to is not empty
            if (txtViewPlayerAgeFrom.Text != "")
            {
                // Try to convert into numbers
                if (!int.TryParse(txtViewPlayerAgeFrom.Text, out ageFrom)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterAgeFrom.SetError(txtViewPlayerAgeFrom, errorMsg);
                    e.Cancel = true;
                }
                else
                {
                    if (txtViewPlayerAgeTo.Text != "") // Age to is greater than age from
                    {
                        ageTo = Convert.ToInt32(txtViewPlayerAgeTo.Text);

                        if (ageTo < ageFrom)
                        {
                            errorMsg = "Age from cannot be lower than age to";
                            errorFilterAgeFrom.SetError(txtViewPlayerAgeFrom, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterAgeFrom.Clear();
                        }

                    }
                    else // Remove error provider
                    {
                        errorFilterAgeFrom.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterAgeFrom.Clear();
            }
        }

        private void txtViewPlayerAgeTo_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int ageFrom;
            int ageTo;

            // If age to is not empty
            if (txtViewPlayerAgeTo.Text != "")
            {

                // Try to convert into numbers              
                if (!int.TryParse(txtViewPlayerAgeTo.Text, out ageTo)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterAgeTo.SetError(txtViewPlayerAgeTo, errorMsg);
                    e.Cancel = true;
                }
                else
                {


                    if (txtViewPlayerAgeFrom.Text != "") // Age to is greater than age from
                    {
                        ageFrom = Convert.ToInt32(txtViewPlayerAgeFrom.Text);

                        if (ageTo < ageFrom)
                        {
                            errorMsg = "Age to cannot be greater than age to";
                            errorFilterAgeTo.SetError(txtViewPlayerAgeTo, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterAgeTo.Clear();
                        }
                    }
                    else // Remove error provider
                    {
                        errorFilterAgeTo.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterAgeTo.Clear();
            }
        }

        private void txtViewPlayerHeightFrom_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int heightFrom;
            int heightTo;

            // If height to is not empty
            if (txtViewPlayerAgeFrom.Text != "")
            {
                // Try to convert into numbers
                if (!int.TryParse(txtViewPlayerHeightFrom.Text, out heightFrom)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterHeightFrom.SetError(txtViewPlayerHeightFrom, errorMsg);
                    e.Cancel = true;
                }
                else
                {
                    if (txtViewPlayerHeightTo.Text != "") // Height to is greater than height from
                    {
                        heightTo = Convert.ToInt32(txtViewPlayerHeightTo.Text);

                        if (heightTo < heightFrom)
                        {
                            errorMsg = "Height from cannot be lower than height to";
                            errorFilterHeightFrom.SetError(txtViewPlayerHeightFrom, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterHeightFrom.Clear();
                        }

                    }
                    else // Remove error provider
                    {
                        errorFilterHeightFrom.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterHeightFrom.Clear();
            }

        }

        private void txtViewPlayerHeightTo_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int heightFrom;
            int heightTo;

            // If height to is not empty
            if (txtViewPlayerHeightTo.Text != "")
            {

                // Try to convert into numbers              
                if (!int.TryParse(txtViewPlayerHeightTo.Text, out heightTo)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterHeightTo.SetError(txtViewPlayerHeightTo, errorMsg);
                    e.Cancel = true;
                }
                else
                {


                    if (txtViewPlayerHeightFrom.Text != "") // Height to is greater than height from
                    {
                        heightFrom = Convert.ToInt32(txtViewPlayerHeightFrom.Text);

                        if (heightTo < heightFrom)
                        {
                            errorMsg = "Height to cannot be greater than height to";
                            errorFilterHeightTo.SetError(txtViewPlayerHeightTo, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterHeightTo.Clear();
                        }
                    }
                    else // Remove error provider
                    {
                        errorFilterHeightTo.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterHeightTo.Clear();
            }
        }

        private void txtViewPlayerWeightFrom_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int weightFrom;
            int weightTo;

            // If Weight to is not empty
            if (txtViewPlayerAgeFrom.Text != "")
            {
                // Try to convert into numbers
                if (!int.TryParse(txtViewPlayerWeightFrom.Text, out weightFrom)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterWeightFrom.SetError(txtViewPlayerWeightFrom, errorMsg);
                    e.Cancel = true;
                }
                else
                {
                    if (txtViewPlayerWeightTo.Text != "") // Weight to is greater than weight from
                    {
                        weightTo = Convert.ToInt32(txtViewPlayerWeightTo.Text);

                        if (weightTo < weightFrom)
                        {
                            errorMsg = "Weight from cannot be lower than weight to";
                            errorFilterWeightFrom.SetError(txtViewPlayerWeightFrom, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterWeightFrom.Clear();
                        }

                    }
                    else // Remove error provider
                    {
                        errorFilterWeightFrom.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterWeightFrom.Clear();
            }
        }

        private void txtViewPlayerWeightTo_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int weightFrom;
            int weightTo;

            // If Weight to is not empty
            if (txtViewPlayerWeightTo.Text != "")
            {

                // Try to convert into numbers              
                if (!int.TryParse(txtViewPlayerWeightTo.Text, out weightTo)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterWeightTo.SetError(txtViewPlayerWeightTo, errorMsg);
                    e.Cancel = true;
                }
                else
                {


                    if (txtViewPlayerWeightFrom.Text != "") // Weight to is greater than weight from
                    {
                        weightFrom = Convert.ToInt32(txtViewPlayerWeightFrom.Text);

                        if (weightTo < weightFrom)
                        {
                            errorMsg = "Weight to cannot be greater than weight from";
                            errorFilterWeightTo.SetError(txtViewPlayerWeightTo, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterWeightTo.Clear();
                        }
                    }
                    else // Remove error provider
                    {
                        errorFilterWeightTo.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterWeightTo.Clear();
            }
        }

        private void txtViewPlayerNumberFrom_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int numberFrom;
            int numberTo;

            // If Number to is not empty
            if (txtViewPlayerAgeFrom.Text != "")
            {
                // Try to convert into numbers
                if (!int.TryParse(txtViewPlayerNumberFrom.Text, out numberFrom)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterNumberFrom.SetError(txtViewPlayerNumberFrom, errorMsg);
                    e.Cancel = true;
                }
                else
                {
                    if (txtViewPlayerNumberTo.Text != "") // Number to is greater than Number from
                    {
                        numberTo = Convert.ToInt32(txtViewPlayerNumberTo.Text);

                        if (numberTo < numberFrom)
                        {
                            errorMsg = "Number from cannot be lower than Nnmber to";
                            errorFilterNumberFrom.SetError(txtViewPlayerNumberFrom, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterNumberFrom.Clear();
                        }

                    }
                    else // Remove error provider
                    {
                        errorFilterNumberFrom.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterNumberFrom.Clear();
            }
        }

        private void txtViewPlayerNumberTo_Validating(object sender, CancelEventArgs e)
        {
            string errorMsg;
            int numberFrom;
            int numberTo;

            // If Number to is not empty
            if (txtViewPlayerNumberTo.Text != "")
            {

                // Try to convert into numbers              
                if (!int.TryParse(txtViewPlayerNumberTo.Text, out numberTo)) // invalid input (Not a number)
                {
                    errorMsg = "Invalid input";
                    errorFilterNumberTo.SetError(txtViewPlayerNumberTo, errorMsg);
                    e.Cancel = true;
                }
                else
                {


                    if (txtViewPlayerNumberFrom.Text != "") // Number to is greater than Number from
                    {
                        numberFrom = Convert.ToInt32(txtViewPlayerNumberFrom.Text);

                        if (numberTo < numberFrom)
                        {
                            errorMsg = "Number to cannot be greater than number from";
                            errorFilterNumberTo.SetError(txtViewPlayerNumberTo, errorMsg);
                            e.Cancel = true;
                        }
                        else
                        {
                            errorFilterNumberTo.Clear();
                        }
                    }
                    else // Remove error provider
                    {
                        errorFilterNumberTo.Clear();
                    }
                }
            }
            else // Remove error provider
            {
                errorFilterNumberTo.Clear();
            }
        }

        // Change update button text when delete checkbox is checked
        private void cbDeletePlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDeletePlayer.Checked)
            {
                btnUpdatePlayer.Text = "Delete";
            }
            else
            {
                btnUpdatePlayer.Text = "Update";
            }
        }

        // Change update button text when delete checkbox is checked
        private void cbDeleteTeam_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDeleteTeam.Checked)
            {
                btnUpdateTeam.Text = "Delete";
            }
            else
            {
                btnUpdateTeam.Text = "Update";
            }
        }

        //remove selected play form lbteam1 and add it to lbteam2
        private void btn_tradeRight_Click(object sender, EventArgs e)
        {
            string sp1 = lbTeam1.SelectedItem.ToString();
            lbTeam2.Items.Add(sp1);
            lbTeam1.Items.Remove(sp1);
        }

        //remove selected play form lbteam2 and add it to lbteam1
        private void btn_tradeLeft_Click(object sender, EventArgs e)
        {
            string sp2 = lbTeam2.SelectedItem.ToString();
            lbTeam1.Items.Add(sp2);
            lbTeam2.Items.Remove(sp2);
        }

        private void btn_confirmTrade_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerPlayerDB db = new SoccerPlayerDB();

            string team1 = cbTeam1.SelectedItem.ToString();
            string team2 = cbTeam2.SelectedItem.ToString();

            int numTeam1 = lbTeam1.Items.Count;
            int numTeam2 = lbTeam2.Items.Count;

            //get team1 player's first name, last name(first name and last name is separte by ' ', but some player has middle name as a part of last name)
            for (int i = 0; i < numTeam1; i++)
            {
                // Create player
                SoccerPlayer player = new SoccerPlayer();

                player.FirstName = lbTeam1.Items[i].ToString().Split(' ')[0];
                player.LastName = lbTeam1.Items[i].ToString().Replace(player.FirstName + ' ', "");
                player.Team = team1;

                int result1 = db.tradePlayer(player);

                if (result1 == 0)
                {
                    MessageBox.Show(player.FirstName + " " + player.LastName + " cannot be traded!", "Trade Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //get team2 player's first name, last name(first name and last name is separte by ' ', but some player has middle name as a part of last name)
            for (int i = 0; i < numTeam2; i++)
            {
                // Create player
                SoccerPlayer player = new SoccerPlayer();

                player.FirstName = lbTeam2.Items[i].ToString().Split(' ')[0];
                player.LastName = lbTeam2.Items[i].ToString().Replace(player.FirstName + ' ', "");
                player.Team = team2;

                int result2 = db.tradePlayer(player);

                if (result2 == 0)
                {
                    MessageBox.Show(player.FirstName + " " + player.LastName + " cannot be traded!", "Trade Player Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            showAllPlayers();
        }
        private void showAllGames()
        {
            SoccerScheduleDB gamesDB = new SoccerScheduleDB();
            SoccerTeamDB teamsDB = new SoccerTeamDB();
            games = gamesDB.getAll();
            teams = teamsDB.getAll();
            // Update Grid Views

            dataGridViewGames.DataSource = games;
            dataGridViewGames.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd";
            // Update ComboBox
            cbHomeTeam.Items.Clear();
            cbVisitorTeam.Items.Clear();

            foreach (DataRow row in teams.Rows)
            {
                cbHomeTeam.Items.Add(row["tname"].ToString());
                cbVisitorTeam.Items.Add(row["tname"].ToString());
                cbFilterGames.Items.Add(row["tname"].ToString());

            }
        }

        private void showRanking()
        {
            SoccerScheduleDB gamesDB = new SoccerScheduleDB();
            ranking = gamesDB.getRanking();
            dataGridViewStandings.DataSource = ranking;
        }

        private void btnAddGame_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerScheduleDB db = new SoccerScheduleDB();
            games = db.getAll();
            int numrows = games.Rows.Count;
            string[] ids = games.AsEnumerable().Select(row => row["gameNo"].ToString()).ToArray();


            int newId = Convert.ToInt32(ids[numrows - 1]) + 1;

            string date = dtpGame.Text;
            string hometeam = cbHomeTeam.Text;
            int homescore = Convert.ToInt32(nudHomeScore.Value);
            string visitorteam = cbVisitorTeam.Text;
            int visitorscore = Convert.ToInt32(nudVisitorScore.Value);
            int points = 3;
            if (homescore == visitorscore)
            {
                points = 1;
            }
            if (hometeam == visitorteam)
            {
                MessageBox.Show("Home team and Guest team cannot be the same", "Update Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SoccerSchedule game = new SoccerSchedule(newId, date, hometeam, homescore, visitorteam, visitorscore,points);

                try
                {
                    int result = db.addGame(game); // Save game

                    if (result > 0)
                    {
                        MessageBox.Show(hometeam + " vs " + visitorteam + " was added successfully!", "Add Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearGameEditFields();
                        showAllGames();
                        showRanking();
                    }
                    else
                    {
                        MessageBox.Show("Game cannot be added!", "Add Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }



        private void clearGameEditFields()
        {
            lblGameId.Text = "";
            dtpGame.Value = DateTime.Now;
            cbHomeTeam.SelectedIndex = -1;
            nudHomeScore.Value = 0;
            cbVisitorTeam.SelectedIndex = -1;
            nudVisitorScore.Value = 0;

        }

        private void dataGridViewGames_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            // Get the index of the cell
            int rowIndex = e.RowIndex;

            // If user do not click on header
            if (rowIndex != -1)
            {
                DataGridViewRow row = dataGridViewGames.Rows[rowIndex];

                // Update text box
                dtpGame.Value = (DateTime)row.Cells[1].Value;
                cbHomeTeam.SelectedItem = row.Cells[2].Value;
                nudHomeScore.Value = Convert.ToInt32(row.Cells[3].Value);
                cbVisitorTeam.SelectedItem = row.Cells[4].Value;
                nudVisitorScore.Value = Convert.ToInt32(row.Cells[5].Value);
                lblGameId.Text = row.Cells[0].Value.ToString();


            }





        }

        private void btnUpdateGame_Click(object sender, EventArgs e)
        {
            // Connect to database
            SoccerScheduleDB db = new SoccerScheduleDB();

            // Get game details
            if (lblGameId.Text == "")
            {
                MessageBox.Show("Please select Game to be updated from GridView", "Update Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int id = Convert.ToInt32(lblGameId.Text);
                string date = dtpGame.Value.ToString("yyyy-MM-dd"); ;
                string hometeam = cbHomeTeam.Text;
                int homescore = Convert.ToInt32(nudHomeScore.Value);
                string visitorteam = cbVisitorTeam.Text;
                int visitorscore = Convert.ToInt32(nudVisitorScore.Value);
                int points = 3;
                if (homescore == visitorscore)
                {
                    points = 1;
                }

                if (hometeam == visitorteam)
                {
                    MessageBox.Show("Home team and Guest team cannot be the same", "Update Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Verify if game exist
                    if (db.getById(id) != null)
                    {

                        SoccerSchedule game = new SoccerSchedule(id, date, hometeam, homescore, visitorteam, visitorscore,points);


                        try
                        {
                            int result = db.updateTeam(game, id); // Save Game

                            if (result > 0)
                            {
                                MessageBox.Show("Game was successfully updated!", "Update Game", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearGameEditFields();
                                showAllGames();
                                showRanking();
                            }
                            else
                            {
                                MessageBox.Show("Game cannot be updated!", "Update Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }



                    else // If game doesn't exist
                    {
                        MessageBox.Show("Please, select Game from Grid View", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDeleteGame_Click(object sender, EventArgs e)
        {
            SoccerScheduleDB db = new SoccerScheduleDB();
            if (lblGameId.Text == "")
            {
                MessageBox.Show("Please select Game to be deleted from GridView", "Delete Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int id = Convert.ToInt32(lblGameId.Text);
                string date = dtpGame.Value.ToString("yyyy-MM-dd"); ;
                string hometeam = cbHomeTeam.Text;
                int homescore = Convert.ToInt32(nudHomeScore.Value);
                string visitorteam = cbVisitorTeam.Text;
                int visitorscore = Convert.ToInt32(nudVisitorScore.Value);
                int points = 3;
                if (homescore == visitorscore)
                {
                    points = 1;
                }
                SoccerSchedule game = new SoccerSchedule(id, date, hometeam, homescore, visitorteam, visitorscore,points);

                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete selected Game? ", "Delete Team", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.No) // Do not delete!
                {

                }
                else
                {
                    try
                    {

                        int result = db.deleteGame(game); // Save team

                        if (result > 0)
                        {
                            MessageBox.Show("Game successfully deleted!", "Delete Game", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            clearGameEditFields();
                            showAllGames();
                            showRanking();
                        }
                        else
                        {
                            MessageBox.Show("Game cannot be deleted!", "Delete Game Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void btnFilterGames_Click(object sender, EventArgs e)
        {
            DataView view = new DataView(games);

            string filter = "";

            if (!string.IsNullOrWhiteSpace(cbFilterGames.Text))
            {
                filter += string.Format("home = '{0}' OR guest ='{0}' ", cbFilterGames.Text);
            }


            if (filter != "")
            {
                

                // Set filter
                view.RowFilter = filter;

                //Update DataGridView
                dataGridViewGames.DataSource = view;

            }
            else // All fields are blank! Show all
            {
                showAllGames();
            }
        }

        private void btnClearFilterGames_Click(object sender, EventArgs e)
        {
            clearGameEditFields();
            showAllGames();
        }
    }
        
        
    
}
