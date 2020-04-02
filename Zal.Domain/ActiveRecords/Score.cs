using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Domain.Models;
using Zal.Domain.Tools;

namespace Zal.Domain.ActiveRecords
{
    public class Score : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal ScoreModel Model;

        public int Id => Model.Id;
        public double Value => Model.Value.Value;
        public bool HasValue => Model.Value.HasValue;
        public int IdUser => Model._Users_Id.Value;
        public string NickName { get; private set; }
        public string Variable { get; private set; }

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

        private UnitOfWork<ScoreUpdateModel> unitOfWork;
        public UnitOfWork<ScoreUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<ScoreUpdateModel>(Model, OnUpdateCommited));

        public Score(ScoreModel model)
        {
            Model = model;
            TrySetNickName();
        }

        internal Score(Game game, int idUser)
        {
            Model = new ScoreModel
            {
                Id_Game = game.Id,
                Value = null,
                _Users_Id = idUser,
            };
            TrySetNickName();
        }

        private void TrySetNickName()
        {
            try
            {
                NickName = Zalesak.Users.GetAvailable(IdUser).NickName;
            }
            catch (Exception)
            {
                NickName = "Exception";
            }
        }

        private async Task<bool> OnUpdateCommited()
        {
            bool isSuccess;
            if (Id != 0)
            {
                isSuccess = await Gateway.UpdateScoreAsync(Model, Zalesak.Session.Token);
            }
            else
            {
                isSuccess = await Gateway.AddScoreAsync(Model, Zalesak.Session.Token);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HasValue"));
            return isSuccess;
        }
    }
}
