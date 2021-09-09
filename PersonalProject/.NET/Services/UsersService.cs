using Sabio.Data.Providers;
using Sabio.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Sabio.Data;
using Sabio.Models.Requests.Addresses;
using Sabio.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Sabio.Services
{
    public class UsersService : IUsersService
    {
        private IDataProvider _data = null;
        private IAuthenticationService<int> _authenticationService;
        public UsersService(IAuthenticationService<int> authSerice, IDataProvider data)
        {
            _data = data;
            _authenticationService = authSerice;
        }

        public async Task<bool> LogInAsync(string email, string password)
        {
            bool isSuccessful = false;

            IUserAuthData response = Get(email, password);

            if (response != null)
            {
                await _authenticationService.LogInAsync(response);
                isSuccessful = true;
            }

            return isSuccessful;
        }

        public void InsertToken(string token, int id)
        {
            string procName = "dbo.UserTokens_Insert";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Token", token);
                collection.AddWithValue("@UserId", id);
            },
            returnParameters: null);
        }
        public void VerifyEmail(string token)
        {
            string procName = "dbo.UserTokens_Update";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Token", token);
            },
           returnParameters: null);
        }

        public void ResetPassword (string token, string password, string passwordConfirm)
        {
            string procName = "dbo.User_PasswordReset";
            string hashedPassword = GenerateHash(password);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Token", token);
                collection.AddWithValue("@Password", hashedPassword);
            },
         returnParameters: null);
        }

        private IUserAuthData Get(string email, string password)
        {

            //make sure the password column can hold long enough string. put it to 100 to be safe
            string passwordFromDb = "";
            UserBase user = null;

            //get user object from db;
            //TESTING
            string procName = "dbo.Users_CheckEmail";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Email", email);

            }, delegate (IDataReader reader, short set)
            {
                user = MapUserBase(reader);
            }
            );

            passwordFromDb = user.Password;
            bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(password, passwordFromDb);
            if (isValidCredentials == false)
            {
                return null;
            }
            //DONE TESTING

            return user;
        }

        public void Update(UserUpdateRequest model)
        {
            string procName = "[dbo].[Users_Update]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(model, collection);
                collection.AddWithValue("@Id", model.Id);
            },
            returnParameters: null);
        }

        public int Create(UserAddRequest userModel)
        {
            int userId = 0;


            //DB provider call to create user and get us a user id
            string procName = "[dbo].[Users_Insert]";

            //be sure to store both salt and passwordHash
            userModel.Password = GenerateHash(userModel.Password);

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(userModel, collection);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);
            },
           returnParameters: delegate (SqlParameterCollection returnCollection)
           {
               object objectId = returnCollection["@Id"].Value;
               int.TryParse(objectId.ToString(), out userId);

           });
            //DO NOT STORE the original password value that the user passed us

            return userId;
        }

        public string GenerateHash(string password)
        {
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            return BCrypt.BCryptHelper.HashPassword(password, salt);
        }

        public User GetById(int id)
        {
            string procName = "[dbo].[Users_SelectById]";
            User user = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
             {
                 user = MapUser(reader);

             }
            );
            return user;

        }

        public List<User> GetAllUsers()
        {
            List<User> listOfUsers = null;
            string procName = "[dbo].[Users_SelectAll]";

            _data.ExecuteCmd(procName, inputParamMapper: null
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    User aUser = MapUser(reader);

                    if (listOfUsers == null)
                    {
                        listOfUsers = new List<User>();
                    }

                    listOfUsers.Add(aUser);
                }
            );


            return listOfUsers;
        }
        public void Delete(int id)
        {
            string procName = "[dbo].[Users_DeleteById]";
            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                collection.AddWithValue("@Id", id);
            },
            returnParameters: null
            );
        }

        public int GetUserByEmail(string email)
        {
            string procName = "dbo.Users_GetUserByEmail";
            int userId = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Email", email);

            }, delegate (IDataReader reader, short set)
            {
                int idx = 0;
                userId = reader.GetInt32(idx++);
            }
         );

            return userId;
        }

        private static User MapUser(IDataReader reader)
        {
            User aUser = new User();

            int startingIdx = 0;

            aUser.Id = reader.GetSafeInt32(startingIdx++);
            aUser.FirstName = reader.GetSafeString(startingIdx++);
            aUser.LastName = reader.GetSafeString(startingIdx++);
            aUser.Email = reader.GetSafeString(startingIdx++);
            aUser.Password = reader.GetSafeString(startingIdx++);
            aUser.AvatarUrl = reader.GetSafeString(startingIdx++);
            aUser.DateAdded = reader.GetSafeUtcDateTime(startingIdx++);
            aUser.DateModified = reader.GetSafeUtcDateTime(startingIdx++);
            return aUser;
        }

        private static UserBase MapUserBase(IDataReader reader)
        {
            UserBase user = new UserBase();

            int idx = 0;

            user.Id = reader.GetSafeInt32(idx++);
            user.Name = reader.GetSafeString(idx++);
            user.Email = reader.GetSafeString(idx++);
            user.Password = reader.GetSafeString(idx++);
            List<Role> roles = reader.DeserializeObject<List<Role>>(idx++);
            user.Roles = roles.Select(r => r.Name).ToList();
            user.TenantId = reader.GetSafeString(idx++);

            return user;
        }

        private static void AddCommonParams(UserAddRequest model, SqlParameterCollection collection)
        {
            collection.AddWithValue("@FirstName", model.FirstName);
            collection.AddWithValue("@LastName", model.LastName);
            collection.AddWithValue("@Email", model.Email);
            collection.AddWithValue("@Password", model.Password);
            collection.AddWithValue("@AvatarUrl", model.AvatarUrl);
            collection.AddWithValue("@TenantId", model.TenantId);
        }

       
    }


}
