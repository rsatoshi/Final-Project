﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.model
{
    class SoccerScheduleDB
    {
        ConnectDB db;
        string table;

        public SoccerScheduleDB()
        {
            db = new ConnectDB();
            table = "schedule";
        }

        // Add a Game
        public int addGame(SoccerSchedule game)
        {
            // Create command
            string command = string.Format("INSERT INTO {0} VALUES({1}, '{2}', '{3}', {4}, '{5}',{6})"
                , table,game.Id, game.Date, game.HomeTeam, game.HomeScore,game.VisitorTeam,game.VisitorScore);

            // Return int
            return db.executeNonQuery(command);
        }
        // Update a Game
        public int updateTeam(SoccerSchedule game, int id)
        {
            // Create command
            string command = string.Format("UPDATE {0} SET date = '{1}', home = '{2}', homeScore = {3}, guest = '{4}', guestScore = {5} WHERE gameNo = {6}", table, game.Date, game.HomeTeam, game.HomeScore, game.VisitorTeam,game.VisitorScore, id);

            // Return int
            return db.executeNonQuery(command);
        }

        // Delete Game
        public int deleteGame(SoccerSchedule game)
        {
            // Create command
            string command = string.Format("DELETE FROM {0} WHERE gameNo = '{1}'", table, game.Id);

            // Return int
            return db.executeNonQuery(command);
        }

        // Get Game by ID
        public SoccerSchedule getById(int Id)
        {
            SoccerSchedule game = new SoccerSchedule();

            // Create command
            string command = string.Format("SELECT * FROM {0} WHERE gameNo = '{1}'", table, Id);

            DataTable result = db.executeReader(command);

            if (result.Rows.Count > 0)
            {
                game.Id = Convert.ToInt32(result.Rows[0]["gameNo"].ToString());
                game.Date = result.Rows[0]["date"].ToString();
                game.HomeTeam = result.Rows[0]["home"].ToString();
                game.HomeScore = Convert.ToInt32(result.Rows[0]["homeScore"].ToString());
                game.VisitorTeam = result.Rows[0]["guest"].ToString();
                game.VisitorScore = Convert.ToInt32(result.Rows[0]["guestScore"].ToString());
                
            }
            else
            {
                game = null;
            }

            return game;
        }
        // Get All games in database
        public DataTable getAll()
        {
            return db.executeReader("SELECT * FROM " + table);
        }
        // Get table schema
        public DataTable getTableSchema()
        {
            return db.getTableSchema(table);
        }
    }
}
