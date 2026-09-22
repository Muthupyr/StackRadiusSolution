using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace StackRadius.Entity
{
    public class MasterUserModel : JSONBaseClass
    {
//int? MasterUserId
//int? UserGroupId
//string UserName
//string UserGroupName
//Boolean? UserStatus
//string Password
//string Title
//string LastName
//string Email
//DateTime? DOB
//string PhoneNo
//string AddressOne
//string AddressTwo
//string AddressThree
//DateTime? LastLoginDate
//string ConfirmPassword
//bool IsLogin
//IList < SelectListItem > UserGroupList
//int? ErrorMessage
//Boolean? IsLoginActive
//string StatusDisplay

        public int? MasterUserId { get; set; }
        public int? UserGroupId { get; set; }
        public string UserName { get; set; }
        public string UserGroupName { get; set; }
        public Boolean? UserStatus { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                    ErrorMessage = "Please Enter Valid Email Address")]
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string PhoneNo { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string AddressThree { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsLogin { get; set; }
        public IList<SelectListItem> UserGroupList { get; set; }
        public int? ErrorMessage { get; set; }
        public Boolean? IsLoginActive { get; set; }
        public string StatusDisplay
        {
            get
            {
                return UserStatus == true ? "Active" : "Inactive";
            }
        }
    }
}