﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using HMS.Data.ViewModels.HouseViewModels;
using HMS.Data.ViewModels;
using HMS.Data.Constants;
using System.Threading.Tasks;
using HMS.Data.Parameters;
using HMS.Data.Responses;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.House;

namespace HMS.Data.Services
{
    public partial interface IHouseService : IBaseService<House>
    {
        List<HouseBaseViewModel> FilterByParameter(string ownerUserId, HouseParameters houseParameters);
        HouseDetailViewModel GetByID(string id);
        Task<ResultResponse> CreateHouseAsync(string userId, CreateHouseViewModel model);
        Task<ResultResponse> DeleteHouseAsync(string id);
        Task<ResultResponse> UpdateHouseAsync(UpdateHouseViewModel model);
        int CountHouses(string userId, HouseParameters houseParameters);
    }
    public partial class HouseService : BaseService<House>, IHouseService
    {
        private readonly IMapper _mapper;
        private readonly IServiceService _serviceService;
        private readonly IHouseInfoService _houseInfoService;
        public HouseService(DbContext dbContext, IHouseRepository repository, IMapper mapper
            , IServiceService serviceService, IHouseInfoService houseInfoService) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _serviceService = serviceService;
            _houseInfoService = houseInfoService;
        }

        public List<HouseBaseViewModel> FilterByParameter(string ownerUserId, HouseParameters houseParameters)
        {
            var houses = Get().Where(h => h.OwnerUserId == ownerUserId && h.IsDeleted == HouseConstants.HOUSE_IS_NOT_DELETED).ProjectTo<HouseBaseViewModel>(_mapper.ConfigurationProvider).ToList();
            var status = houseParameters.Status;
            if(status != null)
            {
                houses = houses.Where(h => h.Status == houseParameters.Status).ToList();
            }
            return houses;
        }

        public HouseDetailViewModel GetByID(string id)
        {
            var house = Get().Where(h => h.Id == id && h.IsDeleted == HouseConstants.HOUSE_IS_NOT_DELETED).ProjectTo<HouseDetailViewModel>(_mapper.ConfigurationProvider).FirstOrDefault();
            return house;
        }

        public async Task<ResultResponse> CreateHouseAsync(string userId, CreateHouseViewModel model)
        {
            var house = _mapper.Map<House>(model);
            house.OwnerUserId = userId;
            house.Id = GenerateHouseID();
            house.Status = HouseConstants.HOUSE_IS_INACTIVE;
            house.IsDeleted = HouseConstants.HOUSE_IS_NOT_DELETED;
            await CreateAsyn(house);
            await _serviceService.CreateDefaultServicesAsync(house.Id);
            return new ResultResponse
            {
                Message = new MessageResult("OK01", new string[] { "House" }),
                IsSuccess = true,
            };
        }

        public async Task<ResultResponse> DeleteHouseAsync(string houseId)
        {
            var houseModel = GetByID(houseId);
            if(houseModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "House" }),
                    IsSuccess = false
                };
            }

            if(houseModel.Status == HouseConstants.HOUSE_IS_ACTIVE)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "House" }),
                    IsSuccess = false
                };
            }

            var house = await GetAsyn(houseId);
            house.IsDeleted = HouseConstants.HOUSE_IS_DELETED;
            Update(house);

            return new ResultResponse
            {
                Message = new MessageResult("OK02", new string[] { "House" }),
                IsSuccess = true
            };
        }

        public int CountHouses(string userId, HouseParameters houseParameters)
        {
            return FilterByParameter(userId, houseParameters).Count;
        }

        private string GenerateHouseID()
        {
            var count = Count();
            string houseId;
            do
            {
                houseId = "H";
                count++;
                if (count < 10)
                {
                    houseId = houseId + "0" + count.ToString();
                }
                else
                {
                    houseId += count.ToString();
                }
            } while (Get(houseId) != null);
            return houseId;
        }

        public async Task<ResultResponse> UpdateHouseAsync(UpdateHouseViewModel model)
        {
            var houseId = model.Id;
            var houseModel = GetByID(houseId);
            if (houseModel == null)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("NF02", new string[] { "House" }),
                    IsSuccess = false
                };
            }

            var houseInfoModelId = houseModel.HouseInfo.Id;
            var result = await _houseInfoService.UpdateHouseInfoAsync(houseInfoModelId, model.HouseInfo);

            if (result.IsSuccess)
            {
                return new ResultResponse
                {
                    Message = new MessageResult("OK03", new string[] { "House" }),
                    IsSuccess = true
                };
            }
            else
            {
                return new ResultResponse
                {
                    Message = new MessageResult("BR05", new string[] { "House" }),
                    IsSuccess = false
                };
            }
        }
    }
}