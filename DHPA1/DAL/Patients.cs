using DHPA1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.IO;

namespace DHPA1.DAL
{
    public class Patients
    {
        /// <summary>
        /// checks if an instance of type Dantist with given email exists in DB
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Boolean EmailExists(string email)
        {
            using (var ctx = new PatientsContext())
            {
                List<Dantist> dantists = ctx.Dantists.ToList();
                foreach (Dantist dantist in dantists)
                {
                    if (dantist.Email == email)
                    { return true; }
                }
            }
            return false;
        }      

        /// <summary>
        /// gets a list of instances of type Dantist from DB
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<Dantist> GetDantists(string userName = "")
        {
            List<Dantist> dantists = null;
            using (var ctx = new PatientsContext())
            {
                dantists = ctx.Dantists.Where(m => m.Login == userName).ToList();
            }
            return dantists;
        }

        /// <summary>
        /// gets a list of instances of type Dantist to be displayed on a page
        /// </summary>
        /// <param name="itemsOnPage"></param>
        /// <param name="pageNumber"></param>
        /// <param name="searchText"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static List<Dantist> GetDantistsOnPage(int itemsOnPage, int pageNumber, string searchText, out int total)
        {
            List<Dantist> students = null;
            using (var ctx = new PatientsContext())
            {
                total = GetAllDantists(searchText).Count(); 
                int pages = total / itemsOnPage; int multiplier = itemsOnPage;
                if (pageNumber == pages + 1)
                { itemsOnPage = total % itemsOnPage; }
                students = ctx.Dantists.Where(s => s.Name.Contains(searchText) || s.Surname.Contains(searchText) || string.IsNullOrEmpty(searchText)).OrderBy(s => s.Name).Skip((pageNumber - 1) * multiplier).Take(itemsOnPage).ToList();
            }
            return students;
        }

        /// <summary>
        /// gets all instances of type Dantist from DB upon given parameters
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static List<Dantist> GetAllDantists(string searchText)
        {
            List<Dantist> dantists = null;
            using (var context = new PatientsContext())
            {
                dantists = context.Dantists.Where(s => s.Name.Contains(searchText) || s.Surname.Contains(searchText) || string.IsNullOrEmpty(searchText)).OrderBy(s => s.Name).ToList();
            }
            return dantists;
        }

        /// <summary>
        /// adds an instance of type Dantist to DB
        /// </summary>
        /// <param name="dantist"></param>
        public static void AddDantist(Dantist dantist)
        {
            using (var ctx = new PatientsContext())
            {
                ctx.Dantists.Add(dantist);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// removes an instance of type Dantist from DB
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteDantist(int? id,string folderPath)
        {
            using (var ctx = new PatientsContext())
            {                
                Dantist dantist = ctx.Dantists.Find(id);
                ctx.Dantists.Remove(dantist);
                ctx.SaveChanges();                
            }
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            } 
        }

        /// <summary>
        /// returns an instance of type Dantist upon given parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Dantist GetDantist(int? id = null, string email = null)
        {
            using (var ctx = new PatientsContext())
            {
                if (id != null)
                {
                    Dantist dantist = ctx.Dantists.Find(id);
                    return dantist;
                }
                else
                {
                    List<Dantist> dantists = ctx.Dantists.Where(m => m.Email == email).ToList();
                    Dantist dantist = dantists[0];
                    return dantist;
                }
            }
        }

        /// <summary>
        /// saves an instance of type Dantist in DB
        /// </summary>
        /// <param name="dantist"></param>
        public static void SaveDantist(Dantist dantist)
        {
            using (var ctx = new PatientsContext())
            {
                ctx.Entry(dantist).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// gets the total number of instances of type Dantist in DB
        /// </summary>
        /// <returns></returns>
        public static int DantistsTotal()
        {
            int number;
            using (var ctx = new PatientsContext())
            { number = ctx.Dantists.Count(); }
            return number;
        }

        /// <summary>
        /// gets a list of instances of type Patient for a certain dantist to be displayed on a page
        /// </summary>
        /// <param name="dantistId"></param>
        /// <param name="itemsOnPage"></param>
        /// <param name="pageNumber"></param>
        /// <param name="searchText"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static List<Patient> GetPatientsOnPage(int dantistId, int itemsOnPage, int pageNumber, string searchText, out int total)
        {
            List<Patient> patients = GetAllPatients(searchText, dantistId).ToList();
            total = patients.Count();
            int pages = total / itemsOnPage; int multiplier = itemsOnPage;
            if (pageNumber == pages + 1)
            { itemsOnPage = total % itemsOnPage; }
            patients = patients.Skip((pageNumber - 1) * multiplier).Take(itemsOnPage).ToList();
            return patients;
        }

        /// <summary>
        /// returns list of instances of type Patient for an instance of type Dantist wiht given id
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="dantistId"></param>
        /// <returns></returns>
        public static List<Patient> GetAllPatients(string searchText, int dantistId)
        {
            using (var ctx = new PatientsContext())
            {
                Dantist dantist = ctx.Dantists.Find(dantistId);
                List<Patient> patients = dantist.Patients.Where(s => s.Name.Contains(searchText) || s.Surname.Contains(searchText) ||s.PhoneNumber.Contains(searchText)||s.Address.CityOrTown.Contains(searchText)|| string.IsNullOrEmpty(searchText)).OrderByDescending(s => s.LastVisitDate).ToList();
                return patients;
            }
        }

        /// <summary>
        /// checks if an instance exists in DB
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Surname"></param>
        /// <param name="DoB"></param>
        /// <param name="dantistId"></param>
        /// <returns></returns>
        public static Boolean PatientExists(string Name, string Surname, DateTime DoB, int dantistId)
        {
            using (var ctx = new PatientsContext())
            {
                Dantist dantist = ctx.Dantists.Find(dantistId);
                if (dantist.Patients.Where(s => s.Name == Name && s.Surname == Surname && s.DOB == DoB).ToList().Count != 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// add an instance of type Patient to an instance of type Dantist
        /// with given id and saves that to DB
        /// </summary>
        /// <param name="dantistId"></param>
        /// <param name="patient"></param>
        public static void AddPatient(int dantistId, Patient patient)
        {
            using (var ctx = new PatientsContext())
            {
                Dantist dantist = ctx.Dantists.Find(dantistId);
                dantist.Patients.Add(patient);
                ctx.Entry(dantist).State = EntityState.Modified;
                ctx.SaveChanges();

            }
        }

        /// <summary>
        /// gets patient with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Patient GetPatient(int? id)
        {
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(id);
                return patient;
            }
        }

        /// <summary>
        /// adds an instance of type Visit to a patient with
        /// given id and saves that to DB
        /// </summary>
        /// <param name="PatientId"></param>
        /// <param name="visit"></param>
        public static void AddVisit(int PatientId, Visit visit) 
        {
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(PatientId);
                patient.LastVisitDate = visit.VisitDate;
                patient.Visits.Add(visit);
                List<Visit> visits = patient.Visits.OrderByDescending(m => m.VisitDate).ToList();
                patient.LastVisitDate = visits[0].VisitDate;
                ctx.Entry(patient).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// gets patients full name without space between name and surname
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetPatientName(int? id)
        {
            string patientName = null;
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(id);
                string[] patientnames = patient.FullName().Split(' ');
                foreach (string item in patientnames)
                {
                    patientName += item;
                }
            }
            return patientName;
        }

        /// <summary>
        /// removes an instance of type Patient with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="folderPath"></param>
        public static void RemovePatient(int? id, string folderPath)
        {
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(id);
                List<Visit> visits = patient.Visits.ToList();
                foreach (Visit item in visits)
                {
                    Visit visit = ctx.Visits.Include(u => u.VisitPictures).Where(p => p.VisitId == item.VisitId).FirstOrDefault();
                    ctx.Visits.Remove(visit);                                     
                }
                List<Tooth> teeth = patient.Teeth.ToList();
                foreach (Tooth item in teeth)
                {
                    Tooth tooth = ctx.Teeth.Include(u => u.Manipulations).Where(p => p.ToothId == item.ToothId).FirstOrDefault();
                    ctx.Teeth.Remove(tooth);
                }
                ctx.Patients.Remove(patient);
                ctx.SaveChanges();
            }
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
           
           
        }

        /// <summary>
        /// returns a list of instance of type Visit for a patient with given id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public static List<Visit> GetVisits(int? patientId)
        {
            List<Visit> visits = new List<Visit>();
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(patientId);
                visits = patient.Visits.OrderByDescending(m => m.VisitDate).ToList();
            }
            return visits;
        }
       
        /// <summary>
        /// adds an instance of type Picture to and instance of type Visit
        /// and saves that to DB
        /// </summary>
        /// <param name="visitId"></param>
        /// <param name="picture"></param>
        public static void NewPicture(int? visitId, Picture picture)
        {
            using (var ctx = new PatientsContext())
            {
                Visit visit = ctx.Visits.Find(visitId);
                visit.VisitPictures.Add(picture);
                ctx.Entry(visit).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// returns a list of instances of type Picture for a certain visit 
        /// where id equals given id
        /// </summary>
        /// <param name="visitId"></param>
        /// <returns></returns>
        public static List<Picture> GetPictures(int? visitId)
        {
            List<Picture> pictures = null;
            using (var ctx = new PatientsContext())
            {
                Visit visit = ctx.Visits.Find(visitId);
                pictures = visit.VisitPictures.ToList();
            }
            return pictures;
        }

        /// <summary>
        /// returns an instance of type Picture from DB with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Picture GetPicture(int? id)
        {
            Picture picture = null;
            using (var ctx = new PatientsContext())
            {
                picture = ctx.Pictures.Find(id);
            }
            return picture;
        }

        /// <summary>
        /// removes an instance of type Picture with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="picturePath"></param>
        public static void RemovePicture(int? id, string picturePath)
        {
            using (var ctx = new PatientsContext())
            {
                Picture picture = ctx.Pictures.Find(id);
                ctx.Pictures.Remove(picture);
                ctx.SaveChanges();
            }
            File.Delete(picturePath);
        }
                     
        /// <summary>
        /// removes an instance of type Visit with given id from DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="folderPath"></param>
        public static void RemoveVisit(int? id, string folderPath)
        {
            using (var ctx = new PatientsContext())
            {
                Visit visit = ctx.Visits.Include(u => u.VisitPictures).Where(p => p.VisitId == id).FirstOrDefault();
                if (folderPath != "")
                {
                    Directory.Delete(folderPath, true);
                }
                ctx.Visits.Remove(visit);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Returns an instance of type Visit with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Visit GetVisit(int? id)
        {
            Visit visit = new Visit();
            using (var ctx = new PatientsContext())
            {
                visit = ctx.Visits.Find(id);
            }
            return visit;
        }

        /// <summary>
        /// Return list of instances of type Tooth for a patient with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<Tooth> GetTeeth(int? id)
        {
            List<Tooth> teeth = null;
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Include(m => m.Teeth).Where(u => u.PatientId == id).FirstOrDefault();
                teeth = patient.Teeth.ToList();
            }
            return teeth;
        }

        /// <summary>
        /// Saves changes DB in list of instance of type Tooth for 
        /// certain patient with given id
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="conditions">list of new conditions</param>
        public static void SaveTeeth(int patientId, List<string> conditions)
        {
            using (var ctx = new PatientsContext())
            {
                Patient patient = ctx.Patients.Find(patientId);
                if (patient.Teeth.Count == conditions.Count)
                {
                    List<Tooth> teeth = patient.Teeth.ToList();
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        teeth[i].Description = conditions[i];
                    }
                    patient.Teeth = teeth;
                    ctx.Entry(patient).State = EntityState.Modified;
                    ctx.SaveChanges();
                }

            }
        }
        
        /// <summary>
        /// Gets an instance of type Tooth from DB upon given id 
        /// </summary>
        /// <param name="id">tooth id</param>
        /// <returns></returns>
        public static Tooth GetTooth(int? id)
        {
            Tooth tooth = null;
            using (var ctx = new PatientsContext())
            {
                tooth = ctx.Teeth.Include(m => m.Manipulations).Where(u => u.ToothId == id).FirstOrDefault();
            }
            return tooth;
        }


        /// <summary>
        /// adds manipulation to tooth with given id and saves it to DB
        /// </summary>
        /// <param name="manipulation">manipulation to add</param>
        /// <param name="toothId">tooth id to which the manipulation is to add</param>
        public static void AddManipulation(Manipulation manipulation, int? toothId)
        {
            using (var ctx = new PatientsContext())
            {
                Tooth tooth = ctx.Teeth.Include(m => m.Manipulations).Where(u => u.ToothId == toothId).FirstOrDefault();
                tooth.Manipulations.Add(manipulation);
                ctx.Entry(tooth).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// adds an instance of <Picture> to manipulation list of <Picture> and saves it to DB 
        /// </summary>
        /// <param name="manipulationId"></param>
        /// <param name="picture"></param>
        public static void AddManipulationImages(int? manipulationId, Picture picture)
        {
            using (var ctx = new PatientsContext())
            {
                Manipulation manipulation = ctx.Manipulations.Include(m => m.ManipulationPictures).Where(u => u.ManipulationId == manipulationId).FirstOrDefault();
                manipulation.ManipulationPictures.Add(picture);
                ctx.Entry(manipulation).State = EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// gets manipulation with given id
        /// </summary>
        /// <param name="id">manipulation id</param>
        /// <returns>manipulation</returns>
        public static Manipulation GetManipulation(int? id)
        {
            Manipulation manipulation = null;
            using (var ctx = new PatientsContext())
            {
                manipulation = ctx.Manipulations.Include(m => m.ManipulationPictures).Where(u => u.ManipulationId == id).FirstOrDefault();
            }
            return manipulation;
        }

        /// <summary>
        /// removes manipulation with given id from DB
        /// </summary>
        /// <param name="id">manipulation id</param>
        /// <param name="folderPath">manipulation path on a disk</param>
        public static void RemoveManipulation(int? id, string folderPath)
        {
            using (var ctx = new PatientsContext())
            {
                Manipulation manipulation = ctx.Manipulations.Include(u => u.ManipulationPictures).Where(p => p.ManipulationId==id).FirstOrDefault();
                if (folderPath != "")
                {
                    Directory.Delete(folderPath, true);
                }
                ctx.Manipulations.Remove(manipulation);
                ctx.SaveChanges();
            }
        }      
    }
}