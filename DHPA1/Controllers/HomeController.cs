using DHPA1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.Specialized;
using System.Configuration;

namespace DHPA1.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// creates var of type register for Login View
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            Register registration = new Register();           
            return View(registration);
        }

        /// <summary>
        /// Checks if Login is correct then checks if password entered is equal to one in DB. If all true
        /// action goes to "ShowPatients". If not indicates about that. 
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(Register registration)
        {               
            List<Dantist> dantists = null;
            dantists = DAL.Patients.GetDantists(registration.Login);
            if (dantists.Count != 0 && dantists.Count == 1)
            {
                string password = PasswordGenny(registration.Password, out byte[] saltout, dantists[0].Salt); 
                if (dantists[0].Password ==password)
                {
                    Session["dantistId"] = dantists[0].DantistId;
                    Session["login"] = dantists[0].Login;
                    if(dantists[0].Login=="admin")
                    { return RedirectToAction("ShowDantists"); }
                    return RedirectToAction("ShowPatients");
                }
                registration.Wrongpassword = true;
                return View(registration);
            }
            registration.UserExists = true;
            return View(registration);
        }

        /// <summary>
        /// Creates a var of type Register and goes to View
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            Register registration = new Register();
            return View(registration);
        }

        /// <summary>
        /// Checks if user with given email is in DB, and if user with given login is in DB and 
        /// indicates about it. If not goes to creates new dantist
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Register registration)
        {
            if(DAL.Patients.EmailExists(registration.Email))
            {
                registration.EmailExists = true;
                return View(registration);
            }
            if (DAL.Patients.GetDantists(registration.Login).Count == 0)
            {
                return RedirectToAction("CreateDantist", registration);
            }
            registration.UserExists = true;
            return View(registration);
        }       

        /// <summary>
        /// Creates var of type Dantist an goes to View
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public ActionResult CreateDantist(Register registration)
        {
            Dantist dantist = new Dantist
            {
                Login = registration.Login,
                Email = registration.Email
            };
            ViewBag.modelnotvalid = false;
            return View(dantist);
        }

        /// <summary>
        /// Creates a dantist and saves it to DB
        /// </summary>
        /// <param name="dantist"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateDantist(Dantist dantist)
        {
            if (ModelState.IsValid)
            {               
                dantist.Password = PasswordGenny(dantist.Password,out byte[] saltout,null); 
                dantist.Salt = saltout;
                DAL.Patients.AddDantist(dantist);
                Session["dantistId"] = dantist.DantistId;
                Session["login"] = dantist.Login;
                return RedirectToAction("ShowPatients");
            }
            ViewBag.modelnotvalid = true;
            return View(dantist);
        }
       
        /// <summary>
        /// Shows patients for certain dantist. Patients are searched accordinly to given parameters
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="items">items per page</param>
        /// <param name="SearchStr">search text to look for in table<patients></param>
        /// <returns>list of patients that complies to given criterias</returns>
        public ActionResult ShowPatients(int pageNumber = 1, int items = 10, string SearchStr = "")
        {
            int dantistId = (int)Session["dantistId"];
            int pagesTotal;
            List<Patient> patients = DAL.Patients.GetPatientsOnPage(dantistId, items, pageNumber, SearchStr, out int total);
            //number of patients that contain given search text
            ViewBag.totalSrch = total;
            if (total % items == 0)
            { pagesTotal = total / items; }
            else { pagesTotal = total / items + 1; }
            //number of pages to display in View
            ViewBag.pages = pagesTotal;
            //items per page
            ViewBag.items = items;
            //current page number
            ViewBag.pageNumber = pageNumber;
            //search text
            ViewBag.searchStr = SearchStr;
            return View(patients);
        }

        /// <summary>
        /// Creates a var of type Patient and returns it to View
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePatient()
        {
            Patient patient = new Patient();
            return View(patient);
        }

        /// <summary>
        /// Creates a patient for given dantist
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreatePatient(Patient patient)
        {
            if (ModelState.IsValid)
            {
                if (Session["dantistId"] != null)
                {
                    int dantistId = (int)Session["dantistId"];
                    //Checks if given patient already exists in DB upon given Name,Surname and DoB 
                    if (DAL.Patients.PatientExists(patient.Name, patient.Surname, patient.DOB, dantistId))
                    {
                        ViewBag.UserExists = true;                        
                        return View(patient);
                    }
                    patient.LastVisitDate = DateTime.Now;
                    // Creates tooth pattern for given patient and sticks to every tooth description
                    //"I"(Intact)
                    List<Tooth> Teeth = new List<Tooth>();
                    for (int i = 18; i > 10; i--)
                    {
                        Tooth tooth = new Tooth { Position = i, Description = "I" };                        
                        Teeth.Add(tooth);
                    }
                    for (int i = 21; i < 29; i++)
                    {
                        Tooth tooth = new Tooth { Position = i, Description = "I" };                        
                        Teeth.Add(tooth);
                    }
                    for (int i = 38; i > 30; i--)
                    {
                        Tooth tooth = new Tooth { Position = i, Description = "I" };                        
                        Teeth.Add(tooth);
                    }
                    for (int i = 41; i < 49; i++)
                    {
                        Tooth tooth = new Tooth { Position = i, Description = "I" };
                        Teeth.Add(tooth);
                    }
                    patient.Teeth = Teeth;
                    DAL.Patients.AddPatient(dantistId, patient);
                    return RedirectToAction("ShowPatients");
                }
            }
            ViewBag.modelnotvalid = true;
            return View(patient);
        }

        /// <summary>
        /// Deletes a patient with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeletePatient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            string patientName = DAL.Patients.GetPatientName(id);
            string folderPath = Server.MapPath(@"~/Images/" + (string)Session["login"] + "/" + patientName);
            DAL.Patients.RemovePatient(id,folderPath);
            Session["patientId"] = null;
            return RedirectToAction("ShowPatients");
        }

        /// <summary>
        /// Logs out dantist
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session["dantistId"] = null;
            return RedirectToAction("Login");
        }


        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Program features description
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.Message = "Dantist notebook";
            return View();
        }

        /// <summary>
        /// Contact information
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {            
            return View();
        }

        /// <summary>
        /// Gets list of dantists from DB upon given criterias
        /// </summary>
        /// <param name="pageNumber">Page to display</param>
        /// <param name="items">Items to dispay per page</param>
        /// <param name="SearchStr">text string to search in table<dantists></param>
        /// <returns>List of dantist that comply to given criterias</returns>
        public ActionResult ShowDantists(int pageNumber = 1, int items = 10, string SearchStr = "")
        {
            int pages;
            List<Dantist> dantists = DAL.Patients.GetDantistsOnPage(items, pageNumber, SearchStr, out int total);
            //total number of dantists in DB
            ViewBag.total = DAL.Patients.DantistsTotal();
            //number of dantists that contain search text
            ViewBag.totalSrch = total;
            if (total % items == 0)
            { pages = total / items; }
            else { pages = total / items + 1; }
            //pages to display in View
            ViewBag.pages = pages;
            //items to display per page
            ViewBag.items = items;
            //current page number
            ViewBag.pageNumber = pageNumber;
            //search text
            ViewBag.searchStr = SearchStr;
            return View(dantists);
        }

        /// <summary>
        /// Removes patient with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDantist(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dantist dantist = DAL.Patients.GetDantist(id);
            string folderPath = Server.MapPath(@"~/Images/" +dantist.Login);
            if(Session["login"].ToString()==dantist.Login)
            { Session["dantistId"] = null; }
            DAL.Patients.DeleteDantist(id,folderPath);
            return RedirectToAction("ShowDantists");
        }

        /// <summary>
        /// Gets the patient from DB whose id equals given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PatientsDetails(int? id)
        {
            if (id == null)
            {
                id = (int)Session["patientId"];
            }
            else
            {
                Session["patientId"] = id;
            }
            Patient patient = DAL.Patients.GetPatient(id);
            return View(patient);
        }

        /// <summary>
        /// Creates var of type CreateVisit and returns it to View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateVisit(int id)
        {
            Session["patientId"] = id;
            CreateVisit visit = new CreateVisit();
            visit.PatientsId = id;
            return View(visit);
        }

        /// <summary>
        /// Creates a visit for patient with given id and saves it in DB. Also creates a folder with
        /// dantist name with subfolder with patient  name with subfolder with visit date in which visit pictures are saved
        /// </summary>
        /// <param name="visit"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateVisit(CreateVisit visit, List<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (Session["dantistId"] != null)
                {
                    Visit currentVisit = new Visit();
                    currentVisit.VisitDate = visit.VisitDate;
                    currentVisit.Description = visit.Description;
                    DAL.Patients.AddVisit(visit.PatientsId, currentVisit);
                    AddVisitImages(currentVisit, files);                   
                    Session["patientId"] = visit.PatientsId;
                    return RedirectToAction("ShowVisits");
                }
                return RedirectToAction("Login");
            }
            return View(visit);
        }

        /// <summary>
        /// Gets a list of visits for a patient with given id from DB and return it to View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowVisits(int? id)
        {
            if (id == null && Session["patientId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? tempId;
            if (id != null)
            { tempId = id; Session["patientId"] = id; }
            else
            { tempId = (int)Session["patientId"]; }

            List<Visit> visits = DAL.Patients.GetVisits(tempId);
            return View(visits);            
        }
        
        /// <summary>
        /// Gets list of pictures for a visit with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult VisitDetails(int? id)
        {
            if (id == null && Session["visitId"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int? tempId;
            if (id != null)
            { tempId = id; Session["visitId"] = id; }
            else
            { tempId = (int)Session["visitId"]; }

            List<Picture> pictures = DAL.Patients.GetPictures(tempId);
            return View(pictures);
        }

        /// <summary>
        /// Removes a visit with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteVisit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visit = DAL.Patients.GetVisit(id);
            string patientName = DAL.Patients.GetPatientName((int)Session["patientId"]);
            string folderPath = Server.MapPath(@"~/Images/" + (string)Session["login"] + "/" + patientName + "/" + visit.VisitDate.ToString("yyyy-MM-dd"));                                   
            DAL.Patients.RemoveVisit(id,folderPath);      
            return RedirectToAction("ShowVisits");
        }

        /// <summary>
        /// Removes a picture with given id as a parameter from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteVisitPicture(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PictureRemover(id);          
            return RedirectToAction("VisitDetails");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddPictures(int? id)
        {
            Session["visitId"] = id;            
            return View();
        }

        /// <summary>
        /// Adds images to visit and saves them in DB
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddPictures(List<HttpPostedFileBase> files)
        {
            if (Session["dantistId"] != null)
            {
                if(files.Count==0)
                {
                    return View();
                }
                Visit visit = DAL.Patients.GetVisit((int)Session["visitId"]);
                AddVisitImages(visit, files);               
                return RedirectToAction("VisitDetails");
            }
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgottenPassword()
        {
            return View();
        }

        /// <summary>
        /// Checks if user with entered email exists in DB and sends a link for renewing password
        /// to user email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ForgottenPassword(string email)
        {
            if (ModelState.IsValid)
            {
                if (DAL.Patients.EmailExists(email))
                {
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("dhpa.service@gmail.com","dantist17"),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                    };
                    NameValueCollection myKeys = ConfigurationManager.AppSettings;
                    string linkPath = myKeys["DomainName"]+"/ResetPassword/?email="+email;  
                    MailMessage mail = new MailMessage("dhpa.service@gmail.com",email,"password reset", "<p>To reset your password follow the link:&nbsp;&nbsp;<a href=" + linkPath+"><strong>Reset password</strong></a></p>");
                    //string body="<p>To change your password press the button:</p></br><button type=\"button\" style=\"background-color:deepskyblue;padding:10px;\" onclick=\"window.open('https://www.w3schools.com/html/');\">Reset password</button>";
                    //mail.Body = body;
                    //mail.Subject = "Password change";
                    mail.IsBodyHtml = true;
                    smtpClient.Send(mail);                   
                }
                else
                {
                    ViewBag.EmailNotExist = true;
                    return View();
                }
                //ViewBag.email = email;
                return View("PasswordRecovery");
            }
            ViewBag.WrongFormat = true;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(string email)
        {
            ResetPassword resetPassword = new ResetPassword();
            resetPassword.Email = email;
            return View(resetPassword);
        }

        /// <summary>
        /// Generates a new password for user with given email and saves that to DB
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(ResetPassword resetPassword)
        {
            Dantist dantist = DAL.Patients.GetDantist(null,resetPassword.Email);
            dantist.Password = PasswordGenny(resetPassword.Password,out byte[]saltout,null);
            dantist.Salt = saltout;
            DAL.Patients.SaveDantist(dantist);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Gets the teeth for a patient with given id from DB 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowTeeth(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session["patientId"] = id;
            List<Tooth>teeth= DAL.Patients.GetTeeth(id);
            Patient patient= DAL.Patients.GetPatient(id);
            ViewBag.fullname = patient.FullName(); 
            return View(model: teeth);
        }

        /// <summary>
        /// Updates the changes to patients teeth and saves them to DB
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShowTeeth(List<string>conditions)
        {                              
                DAL.Patients.SaveTeeth((int)Session["patientId"],conditions);
                        
            return RedirectToAction("PatientsDetails",new { id= (int)Session["patientId"]});
        }

        /// <summary>
        /// Gets the all manipuations for a tooth with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowManipulations(int? id) 
        {
            if (id == null&& Session["toothId"]==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int tempId;
            if (id != null) { tempId =(int)id; Session["toothId"] = id; } else { tempId = (int)Session["toothId"]; }
            Tooth tooth = DAL.Patients.GetTooth(tempId);
            List<Manipulation> manipulations = tooth.Manipulations.OrderByDescending(m=>m.ManipulationDate).ToList();
            return View(manipulations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateManipulation(int? id)
        {
            Session["toothId"] = id;
            Manipulation manipulation = new Manipulation();            
            return View(manipulation);            
        }

        /// <summary>
        /// Creates new manipulation for a tooth with given id and saves it to DB
        /// </summary>
        /// <param name="manipulation"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateManipulation(Manipulation manipulation, List<HttpPostedFileBase> files)  
        {
            if (ModelState.IsValid)
            {
                if (Session["dantistId"] != null)
                {
                    DAL.Patients.AddManipulation(manipulation, (int)Session["toothId"]);
                    AddManipulationImages(manipulation, files);                    
                    Session["manipulationId"] = manipulation.ManipulationId;
                    return RedirectToAction("ShowManipulations");
                }
                return RedirectToAction("Login");
            }
            return View(manipulation);
        }

        /// <summary>
        /// Adds images to manipulation with given id and saves them to DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddImages(int? id)
        {
            Session["manipulationId"] = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddImages(List<HttpPostedFileBase> files)
        {
            if (Session["dantistId"] != null)
            {
                if (files.Count == 0)
                {
                    return View();
                }
                Manipulation manipulation = DAL.Patients.GetManipulation((int)Session["manipulationId"]);
                AddManipulationImages(manipulation, files);               
                return RedirectToAction("ShowManipulations");
            }
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Gets all images for a manipulation with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ManipulationDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session["manipulationId"] = id;
            Manipulation manipulation = DAL.Patients.GetManipulation(id);
            List<Picture> pictures =manipulation.ManipulationPictures.ToList();
            return View(pictures);
        }

        /// <summary>
        /// Deletes a manipulation with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteManipulation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manipulation manipulation = DAL.Patients.GetManipulation(id);
            string patientName = DAL.Patients.GetPatientName((int)Session["patientId"]);
            Tooth tooth = DAL.Patients.GetTooth((int)Session["toothId"]);
            string toothName = "Tooth_" + tooth.Position.ToString();
            string folderPath = Server.MapPath(@"~/Images/" + (string)Session["login"] + "/" + patientName + "/" + toothName + "/" + manipulation.ManipulationDate.ToString("yyyy-MM-dd") + "/");            
            DAL.Patients.RemoveManipulation(id, folderPath);
            return RedirectToAction("ShowManipulations");
        }

        /// <summary>
        /// Deletes an image with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteManipulationPicture(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PictureRemover(id);          
            return RedirectToAction("ManipulationDetails",new {id= (int)Session["manipulationId"] });
        }

        /// <summary>
        /// Creates a hashed password based on input parameters
        /// </summary>
        /// <param name="password"></param>
        /// <param name="saltout"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public string PasswordGenny(string password, out byte[] saltout, byte[]salt=null)
        {
            if (salt is null)
            {
                // generate a 128-bit salt using a secure PRNG
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }          
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            saltout = salt;
            return hashed;
        }

        /// <summary>
        /// Deletes an image with given id from DB
        /// </summary>
        /// <param name="id"></param>
        public void PictureRemover(int? id)
        {            
            Picture picture = DAL.Patients.GetPicture(id);
            string picturePath = Server.MapPath(picture.PicturePath);
            DAL.Patients.RemovePicture(id, picturePath);            
        }
               
        /// <summary>
        /// Adds images to a given manipulation and saves them to DB
        /// </summary>
        /// <param name="manipulation"></param>
        /// <param name="files"></param>
        public void AddManipulationImages(Manipulation manipulation,List<HttpPostedFileBase>files) 
        {
            string patientName = DAL.Patients.GetPatientName((int)Session["patientId"]);
            Tooth tooth = DAL.Patients.GetTooth((int)Session["toothId"]);
            string toothName = "Tooth_" + tooth.Position.ToString();
            string path = "~/Images/" + (string)Session["login"] + "/" + patientName + "/" + toothName + "/" + manipulation.ManipulationDate.ToString("yyyy-MM-dd") + "/";
            AddImagesHelper manipulationImages = DAL.Patients.AddManipulationImages;
            ImageSaver(manipulationImages, manipulation.ManipulationId, path, files);
        }

        /// <summary>
        /// Adds images to a given visit and saves them to DB
        /// </summary>
        /// <param name="visit"></param>
        /// <param name="files"></param>
        public void AddVisitImages(Visit visit,List<HttpPostedFileBase>files)
        {
            string patientname = DAL.Patients.GetPatientName((int)Session["patientId"]);
            string path = "~/Images/" + (string)Session["login"] + "/" + patientname + "/" + visit.VisitDate.ToString("yyyy-MM-dd") + "/";
            AddImagesHelper visitImages = DAL.Patients.NewPicture;
            ImageSaver(visitImages, visit.VisitId, path, files);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="picture"></param>
        public delegate void AddImagesHelper(int? id, Picture picture);

        /// <summary>
        /// Saves images in DB 
        /// </summary>
        /// <param name="addImages"></param>
        /// <param name="id"></param>
        /// <param name="path"></param>
        /// <param name="files"></param>
        public void ImageSaver(AddImagesHelper addImages,int id,string path,List<HttpPostedFileBase>files)
        {
            string folderPath = Server.MapPath(path);
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            //iterating through multiple file collection   
            foreach (HttpPostedFileBase file in files)
            {
                //Checking file is available to save.  
                if (file != null)
                {
                    Picture picture = new Picture();
                    var InputFileName = Path.GetFileName(file.FileName);
                    var ServerSavePath = Path.Combine(folderPath, InputFileName);
                    //Save file to server folder  
                    file.SaveAs(ServerSavePath);
                    picture.PicturePath = path + InputFileName;
                    addImages(id,picture);
                }
            }

        }
    }
}