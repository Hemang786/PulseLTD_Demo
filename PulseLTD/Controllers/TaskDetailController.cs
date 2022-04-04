using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PulseLTD.Common;
using PulseLTD.Models;

namespace PulseLTD.Controllers
{
    public class TaskDetailController : Controller
    {
        #region Private Variable
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        #endregion

        #region TaskDetailsGrid
        /// <summary>
        /// GET: TaskDetail
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: Getting data from database and display data in datatable for grid.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDetails()
        {
            var list = db.TaskDetails.ToList();
            return new JsonResult() { Data = new { data = list, recordsTotal = list.Count, recordsFiltered = list.Count, draw = Request["draw"] }, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        #endregion

        #region SaveTaskDetails
        /// <summary>
        /// GET: AddTaskDetail
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new TaskDetail());
        }

        /// <summary>
        /// POST: save task details in database
        /// </summary>
        /// <param name="taskModel">pass all fields in model</param>
        /// <param name="postedFile">pass uploaded image</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(TaskDetail taskModel, HttpPostedFileBase postedFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var list = db.TaskDetails.Select(x=>x.Title).ToList();
                    if (list != null && list.Count > 0)
                    {
                        var isExistsTitle = list.FirstOrDefault(x => x.ToLower() == taskModel.Title.ToLower());
                        if (!string.IsNullOrEmpty(isExistsTitle))
                        {
                            return Json(new { Message = MessageHelper.AlreadyExists, Type = Enums.NotifyType.ErrorMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    string base64String;
                    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                    {
                        byte[] bytes = br.ReadBytes(postedFile.ContentLength);
                        base64String = Convert.ToBase64String(bytes);
                    }

                    taskModel.ImageText = base64String;
                    taskModel.DateConverted = DateTime.UtcNow;
                    db.TaskDetails.Add(taskModel);
                    db.SaveChanges();

                    return Json(new { Message = MessageHelper.SaveData, Type = Enums.NotifyType.SuccessMessage }, JsonRequestBehavior.AllowGet);
                }
                var message = string.Join(" <br/> ", ModelState.Values
                                                       .SelectMany(v => v.Errors)
                                                       .Select(e => e.ErrorMessage));
                return Json(new { Message = message, Type = Enums.NotifyType.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.ToString(), Type = Enums.NotifyType.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region DownloadZip
        /// <summary>
        /// Download zip folder with selected images.
        /// </summary>
        /// <param name="ids">pass the parameter for selected row</param>
        /// <returns></returns>
        public ActionResult DownloadZipFile(string ids)
        {

            if (string.IsNullOrEmpty(ids))
            {
                return Json(new { Message = MessageHelper.SelectRows, Type = Enums.NotifyType.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            var list = db.TaskDetails.ToList();

            using (var compressedFileStream = new MemoryStream())
            {
                //Create an archive and store the stream in memory.
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, true))
                {
                    var selectedRows = ids.Split(',');
                    foreach (var item in selectedRows)
                    {
                        var currentData = list.FirstOrDefault(x => x.Id == Convert.ToInt32(item));
                        if (currentData != null)
                        {
                            //Create a zip entry for each attachment
                            var zipEntry = zipArchive.CreateEntry(currentData.ImageName);

                            //Get the stream of the attachment
                            using (var originalFileStream = new MemoryStream(Convert.FromBase64String(currentData.ImageText)))
                            using (var zipEntryStream = zipEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = DateTime.UtcNow.ToString() + ".zip" };
            }
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
