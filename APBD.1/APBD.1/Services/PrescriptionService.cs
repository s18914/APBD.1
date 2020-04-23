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
            if (request.DueDate <= request.Date)
            {
                return null;
            }

            using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("DB_CONNECTION")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "insert into Prescription(date, duedate, idpatient, iddoctor) values (@date, @dueDate, @patient, @doctor);"
                    + "select SCOPE_IDENTITY()";

                command.Parameters.AddWithValue("date", request.Date);
                command.Parameters.AddWithValue("dueDate", request.DueDate);
                command.Parameters.AddWithValue("patient", request.IdPatient);
                command.Parameters.AddWithValue("doctor", request.IdDoctor);


                connection.Open();
                var id = Convert.ToInt32(command.ExecuteScalar());

                return new AddPrescriptionResponse
                {
                    IdPrescription = id,
                    Date = request.Date,
                    DueDate = request.DueDate,
                    IdDoctor = request.IdDoctor,
                    IdPatient = request.IdPatient
                };
            }
        }
    }


}

