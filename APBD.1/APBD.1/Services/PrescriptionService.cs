using APBD._1.DTOs.Requests;
using APBD._1.DTOs.Responses;
using APBD._1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD._1.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        string ConString = "Data Source=db-mssql;Initial Catalog=s18914;Integrated Security=True";

        public List<Prescription> GetPrescriptions(string LastName)
        {
            var list = new List<Prescription>();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
               
                com.Connection = con;

                com.CommandText = $"select IdPrescription, Date, DueDate, p.LastName, d.LastName from Prescription " +
                    $"inner join Patient p where Prescription.IdPatient = Patient.IdPatient " +
                    $"inner join Doctor on Doctor.IdDoctor = Prescription.IdPrescription order by Date DESC";
                if (LastName != null)
                {
                    com.CommandText = $"select IdPrescription, Date, DueDate, p.LastName, d.LastName from Prescription " +
                                        $"inner join Patient p where Prescription.IdPatient = Patient.IdPatient " +
                                        $"inner join Doctor on Doctor.IdDoctor = Prescription.IdPrescription " +
                                        $"where d.LastName = @lastName order by Date DESC";
                    com.Parameters.AddWithValue("lastName", LastName);
                }
                try
                {
                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        var pre = new Prescription();
                        pre.IdPrescription = Int32.Parse(dr["IdPrescription"].ToString());
                        pre.Date = dr["Date"].ToString();
                        pre.DueDate = dr["DueDate"].ToString();
                        pre.PatientLastName = dr["p.LastName"].ToString();
                        pre.DoctorLastName = dr["d.LastName"].ToString();
                        list.Add(pre);
                    }
                }
                catch (SqlException exc)
                {
                    return null;
                }
            }

            return list;

        }


        public AddPrescriptionResponse AddPrescription(AddPrescriptionRequest request)
        {
            var an = new AddPrescriptionResponse();

            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                con.Open();
                com.Connection = con;
                var tran = con.BeginTransaction();

                //czy doktor istnieje
                com.CommandText = "select IdDoctor from Doctor where IdDoctor = @IdDoctor";
                com.Parameters.AddWithValue("IdDoctor", request.IdDoctor);


                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    tran.Rollback();
                    return null;
                }
                //czy pacjent istnieje
                com.CommandText = "select IdPatient from Patient where IdPatient = @IdPatient";
                com.Parameters.AddWithValue("IdPatient", request.IdPatient);


                if (!dr.Read())
                {
                    tran.Rollback();
                    return null;
                }


                string date = (string)dr["date"];
                string dueDate = (string)dr["dueDate"];
                //dodaje zwierze
                com.CommandText = "insert into Animal() values ...";

                com.ExecuteNonQuery();


                tran.Commit();

            }

            // var response = new AddAnimalResponse();
            //response.LastName = st.Last name //itd...
            //potem return Ok(response)

            return an;
        }
    }


}

