using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Zal.Bridge.Gateways;
using Zal.Bridge.Models;
using Zal.Domain.Models;
using Zal.Domain.Tools;
using Zal.Bridge.Models.ApiModels;

namespace Zal.Domain.ActiveRecords
{
    public class Gallery : IActiveRecord
    {
        private GalleryModel Model;
        private List<string> images;

        public int Id => Model.Id;
        public int Year => Model.Year;
        public string Name => Model.Name;
        public string File => Model.File;
        public DateTime Date => Model.Date;

        private static GalleryGateway gateway;
        private static GalleryGateway Gateway => gateway ?? (gateway = new GalleryGateway());

        private UnitOfWork<GalleryUpdateModel> unitOfWork;
        public UnitOfWork<GalleryUpdateModel> UnitOfWork => unitOfWork ?? (unitOfWork = new UnitOfWork<GalleryUpdateModel>(Model, OnUpdateCommited));

        private Task<bool> OnUpdateCommited()
        {
            return Gateway.UpdateAsync(Model, Zalesak.Session.Token);
        }

        private async Task<IEnumerable<string>> ImagesLazyLoad()
        {
            if (images == null)
            {
                images = (await Gateway.GetAsync(Id)).ToList();
            }
            return images;
        }

        internal Gallery(GalleryModel model)
        {
            Model = model;
        }

        public async Task<bool> Upload(string imageName, byte[] rawImage, bool isMain = false)
        {
            var model = new ImageUploadModel
            {
                Id = Id,
                SetAsMain = isMain,
                Name = imageName,
            };
            bool isUploaded = await Gateway.UploadImage(model, rawImage, "Zalesak.Session.Token");
            if (isUploaded) images.Add(imageName);
            return isUploaded;
        }

        internal static async Task<IEnumerable<Gallery>> GetAll()
        {
            var rawModels = await Gateway.GetAllAsync();
            var galleries = rawModels.Select(model => new Gallery(model));
            return galleries;
        }

        internal static async Task<Gallery> Add(string name, int year, DateTime date)
        {
            var model = new GalleryModel
            {
                Name = name,
                Year = year,
                Date = date,
            };
            var respond = await Gateway.AddAsync(model, "galToken");
            if (respond.Id != -1)
            {
                model.Id = respond.Id;
                model.File = respond.FileName;
            }
            return new Gallery(model);
        }
    }
}
