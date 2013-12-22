﻿using Magenic.BadgeApplication.Common.DTO;
using Magenic.BadgeApplication.Common.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Magenic.BadgeApplication.DataAccess.EF
{
    public class SubmitActivityDAL : ISubmitActivityDAL
    {
        public async Task<ISubmitActivityDTO> GetActivitySubmissionByIdAsync(int activitySubmissionId)
        {
            using (var ctx = new Entities())
            {
                ctx.Database.Connection.Open();
                var badgeList = await (from t in ctx.ActivitySubmissions
                    where t.ActivitySubmissionId == activitySubmissionId
                    select new SubmitActivityDTO
                    {
                        Id = t.ActivitySubmissionId,
                        ActivityId = t.ActivityId,
                        ActivitySubmissionDate = t.SubmissionDate,
                        ApprovedByUserName = t.SubmissionApprovedADName,
                        Notes = t.SubmissionDescription,
                        Status = (Common.Enums.ActivitySubmissionStatus) t.SubmissionStatusId,
                        UserName = t.EmployeeADName
                    }).ToArrayAsync();

                var badge = badgeList.SingleOrDefault();

                return badge;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public ISubmitActivityDTO Update(ISubmitActivityDTO data)
        {
            using (var ctx = new Entities())
            {
                ctx.Database.Connection.Open();
                var saveActivitySubmission = LoadData(data);
                ctx.ActivitySubmissions.Attach(saveActivitySubmission);
                var objectState = ((IObjectContextAdapter)ctx).ObjectContext.ObjectStateManager;
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("ActivityId");
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("EmployeeADName");
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("SubmissionStatusId");
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("SubmissionApprovedADName");
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("SubmissionDate");
                objectState.GetObjectStateEntry(saveActivitySubmission).SetModifiedProperty("SubmissionDescription");

                ctx.SaveChanges();
                data.Id = saveActivitySubmission.ActivitySubmissionId;
            }
            return data;
        }

        private static ActivitySubmission LoadData(ISubmitActivityDTO data)
        {
            var activitySubmission = new ActivitySubmission
            {
                ActivitySubmissionId = data.Id,
                ActivityId = data.ActivityId,
                EmployeeADName = data.UserName,
                SubmissionStatusId = (int)data.Status,
                SubmissionApprovedADName = data.ApprovedByUserName.Length == 0? null: data.ApprovedByUserName,
                SubmissionDate = data.ActivitySubmissionDate,
                SubmissionDescription = data.Notes
            };
            return activitySubmission;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public ISubmitActivityDTO Insert(ISubmitActivityDTO data)
        {
            using (var ctx = new Entities())
            {
                ctx.Database.Connection.Open();
                var saveActivitySubmission = LoadData(data);
                ctx.ActivitySubmissions.Add(saveActivitySubmission);

                ctx.SaveChanges();
                data.Id = saveActivitySubmission.ActivitySubmissionId;
            }
            return data;
        }

        public void Delete(int activitySubmissionId)
        {
            using (var ctx = new Entities())
            {
                ctx.Database.Connection.Open();
                var deleteActivitySubmission = new ActivitySubmission
                {
                    ActivitySubmissionId = activitySubmissionId
                };
                ctx.ActivitySubmissions.Attach(deleteActivitySubmission);
                ctx.ActivitySubmissions.Remove(deleteActivitySubmission);
                ctx.SaveChanges();
            }
        }
    }
}