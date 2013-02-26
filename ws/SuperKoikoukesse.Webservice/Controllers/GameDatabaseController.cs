﻿using Pixelnest.Common.Log;
using SuperKoikoukesse.Webservice.Core.Dao;
using SuperKoikoukesse.Webservice.Core.Games;
using SuperKoikoukesse.Webservice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperKoikoukesse.Webservice.Controllers
{
    [Authorize]
    public class GameDatabaseController : Controller
    {
        private GameInfoDb m_gamesDb;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (m_gamesDb == null)
            {
                string dbPath = Server.MapPath(ConfigurationManager.AppSettings["GAME_DB_PATH"].ToString());

                m_gamesDb = new GameInfoDb(dbPath);
                if (m_gamesDb.IsNew)
                {
                    m_gamesDb.Save();
                }
            }
        }

        private static int pageSize = 100;

        public ActionResult Index(int page = 1)
        {
            if (page < 1) page = 1;
            List<GameInfo> gameDb = m_gamesDb.ReadAll();

            GameDatabaseModel model = new GameDatabaseModel();
            model.Page = page;
            model.MaxPage = gameDb.Count / pageSize;

            if (page > model.MaxPage) page = model.MaxPage;

            // Apply pagination
            model.Games = gameDb.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(model);
        }

        /// <summary>
        /// Import CSV database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImportCSV(HttpPostedFileBase file)
        {
            ImportResultModel model = new ImportResultModel();
            Exception exception = null;
            int gameCount = 0;

            if (file != null && file.ContentLength > 0 && Path.GetExtension(file.FileName).ToLower().EndsWith("csv"))
            {
                // Get the uploaded file
                string csvContent = string.Empty;
                using (StreamReader reader = new StreamReader(file.InputStream))
                {
                    csvContent = reader.ReadToEnd();
                }

                // Read it
                if (string.IsNullOrEmpty(csvContent))
                {
                    model.IsSuccess = false;
                    model.Message = "File was empty!";
                }
                else
                {
                    // Save the old file
                    m_gamesDb.Backup();
                    m_gamesDb.DeleteAll();

                    // CSV
                    // Format : GameId, image, pal, us, support, genre, editor, year, removed
                    int lineNumber = 0;
                    foreach (string line in csvContent.Split('\n'))
                    {
                        if (string.IsNullOrEmpty(line) == false)
                        {
                            lineNumber++;
                            string[] linePart = line.Split(';');

                            try
                            {
                                int gameId = Convert.ToInt32(linePart[0]);
                                string imagePath = linePart[1];
                                string titlePal = linePart[2];
                                string titleUs = linePart[3];
                                string platform = linePart[4].ToLower();
                                string genre = linePart[5].ToLower();
                                string publisher = linePart[6].ToLower();
                                int year = Convert.ToInt32(linePart[7]);

                                GameInfo game = new GameInfo()
                                {
                                    GameId = gameId,
                                    ImagePath = imagePath,
                                    TitlePAL = titlePal,
                                    TitleUS = titleUs,
                                    Platform = platform,
                                    Genre = genre,
                                    Publisher = publisher,
                                    Year = year,
                                };

                                m_gamesDb.Add(game);
                                gameCount++;
                            }
                            catch (Exception e)
                            {
                                // We skip the first line
                                if (lineNumber > 1)
                                {
                                    Logger.LogException(LogLevel.Error, "ImportCSV", e);
                                    exception = new ArgumentException("Error CSV parser line " + lineNumber + ". ", e);

                                    // Stop parsing
                                    break;
                                }
                            }
                        }
                    }
                }

                if (exception != null)
                {
                    model.Exception = exception;
                    m_gamesDb.Restorebackup();

                    model.Message = "Error happended during process. Database has been restored from previous backup.";
                    model.IsSuccess = false;
                }
                else
                {
                    m_gamesDb.Save();

                    model.Message = "Successfully imported " + gameCount + " games.";
                    model.IsSuccess = true;
                }
            }
            else
            {
                model.IsSuccess = false;
                model.Message = "No .csv file found!";
            }

            return View(model);
        }
    }
}
