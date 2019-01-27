using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zal.Bridge.Models;
using Zal.Bridge.Models.ApiModels;
using Zal.Bridge.Tools;

namespace Zal.Bridge
{
    public class UserGateway : Gateway {

        public UserGateway() : base(API.ENDPOINT.USERS) { }

        public Task<UserModel> GetAsync(int id) {
            return SendRequestFor<UserModel>(API.METHOD.GET, id);
        }

        public Task<AllRespondModel<UserModel>> GetMoreAsync(UserRequestModel model) {
            return SendRequestFor<AllRespondModel<UserModel>>(API.METHOD.GET_MORE, model);
        }

        public async Task<FullChangesRespondModel<UserModel>> GetAllChangedAsync(UserChangesRequestModel model, string token) {//todo token?
            var respond = await SendRequestForNullable<FullChangesRespondModel<UserModel>>(API.METHOD.GET_CHANGED, model);
            return respond ?? new FullChangesRespondModel<UserModel>();
        }

        public async Task<bool> AddAsync(UserModel model, string token) {//nastaví správně id u modelu?
            model.Id = await SendRequestFor<int>(API.METHOD.ADD, model, token);
            return model.Id != -1;
        }

        public Task<bool> DeleteAsync(int id, string token) {
            return SendRequestFor<bool>(API.METHOD.DELETE, id, token);
        }

        public Task<bool> AddBadgeAsync(User_BadgeModel model) {
            return SendRequestFor<bool>(API.METHOD.ADD_BADGE_TO, model);
        }

        //todo bodíky

        public bool BecomeMember(UserModel user) {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(UserModel user, string token) {
            return SendRequestFor<bool>(API.METHOD.UPDATE, user, token);
        }

        public bool Update(string email, bool isPaid) {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserModel>> GetAsync(IEnumerable<int> ids) {
            throw new NotImplementedException();
        }

        public Task<bool> UploadProfileImage(ImageUploadModel model, byte[] rawImage, string token)
        {
            return SendImageUploadRequestFor<bool>(API.METHOD.UPLOAD_IMAGE, rawImage, model, token);
        }


        /*public Collection<Uzivatel> getParticipatingAt(Akce akce) {
            SqlCommand command = db.CreateCommand(SQL_SELECT_USERS_AT_ACTION);
            command.Parameters.Add(SetParam("@id_akce", akce.Id);
            SqlDataReader reader = db.Select(command);
            return ReadAllFrom(reader);
        }*/

        //public  Collection<Uzivatel> getParticipatingAt(Akce akce, Druzina druzina)
        //{
        //    SqlCommand command = db.CreateCommand(SQL_SELECT_USERS_AT_ACTION + WHERE_DRUZINA);
        //    command.Parameters.Add(SetParam("@id_akce", akce.Id);
        //    command.Parameters.Add(SetParam("@id_druzina", druzina.Id);
        //    SqlDataReader reader = db.Select(command);
        //    return ReadAllFrom(reader);
        //}
    }
}
