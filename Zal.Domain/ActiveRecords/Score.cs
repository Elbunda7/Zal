using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Domain.Models;
using Zal.Domain.Tools;

namespace Zal.Domain.ActiveRecords
{
    public class Score
    {
        private ScoreModel Model;

        public int Id => Model.Id;
        public string Value => Model.Value;
        public bool HasValue => Value != null;
        public int IdUser => Model._Users_Id.Value;
        public string NickName { get; private set; }
        public string Variable { get; private set; }

        private static GameGateway gateway;
        private static GameGateway Gateway => gateway ?? (gateway = new GameGateway());

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

        private UnitOfWork<ScoreUpdateModel> unitOfWork;
        public UnitOfWork<ScoreUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<ScoreUpdateModel>(Model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited()
        {
            if (Id != 0)
            {
                return Gateway.UpdateScoreAsync(Model, Zalesak.Session.Token);
            }
            else
            {
                return Gateway.AddScoreAsync(Model, Zalesak.Session.Token);
            }
        }
    }
}
