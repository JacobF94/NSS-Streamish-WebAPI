using Streamish.Controllers;
using Streamish.Models;
using Streamish.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Streamish.Tests
{
    public class UserProfileControllerTests
    {
        [Fact]
        public void Get_Returns_All_Videos()
        {
            // Arrange 
            var videoCount = 20;
            var videos = CreateTestVideos(videoCount);

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            // Act 
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualVideos = Assert.IsType<List<Video>>(okResult.Value);

            Assert.Equal(videoCount, actualVideos.Count);
            Assert.Equal(videos, actualVideos);
        }

        [Fact]
        public void Get_By_Id_Returns_NotFound_When_Given_Unknown_id()
        {
            // Arrange 
            var videos = new List<Video>(); // no videos

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            // Act
            var result = controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_By_Id_Returns_Video_With_Given_Id()
        {
            // Arrange
            var testVideoId = 99;
            var videos = CreateTestVideos(5);
            videos[0].Id = testVideoId; // Make sure we know the Id of one of the videos

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            // Act
            var result = controller.Get(testVideoId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualVideo = Assert.IsType<Video>(okResult.Value);

            Assert.Equal(testVideoId, actualVideo.Id);
        }

        [Fact]
        public void Post_Method_Adds_A_New_Video()
        {
            // Arrange 
            var videoCount = 20;
            var videos = CreateTestVideos(videoCount);

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            // Act
            var newVideo = new Video()
            {
                Description = "Description",
                Title = "Title",
                Url = "http://youtube.url?v=1234",
                DateCreated = DateTime.Today,
                UserProfileId = 999,
                UserProfile = CreateTestUserProfile(999),
            };

            controller.Post(newVideo);

            // Assert
            Assert.Equal(videoCount + 1, repo.InternalData.Count);
        }

        [Fact]
        public void Put_Method_Returns_BadRequest_When_Ids_Do_Not_Match()
        {
            // Arrange
            var testVideoId = 99;
            var videos = CreateTestVideos(5);
            videos[0].Id = testVideoId; // Make sure we know the Id of one of the videos

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            var videoToUpdate = new Video()
            {
                Id = testVideoId,
                Description = "Updated!",
                Title = "Updated!",
                UserProfileId = 99,
                DateCreated = DateTime.Today,
                Url = "http://some.url",
            };
            var someOtherVideoId = testVideoId + 1; // make sure they aren't the same

            // Act
            var result = controller.Put(someOtherVideoId, videoToUpdate);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_Method_Updates_A_Video()
        {
            // Arrange
            var testVideoId = 99;
            var videos = CreateTestVideos(5);
            videos[0].Id = testVideoId; // Make sure we know the Id of one of the videos

            var repo = new InMemoryVideoRepository(videos);
            var controller = new VideoController(repo);

            var videoToUpdate = new Video()
            {
                Id = testVideoId,
                Description = "Updated!",
                Title = "Updated!",
                UserProfileId = 99,
                DateCreated = DateTime.Today,
                Url = "http://some.url",
            };

            // Act
            controller.Put(testVideoId, videoToUpdate);

            // Assert
            var videoFromDb = repo.InternalData.FirstOrDefault(p => p.Id == testVideoId);
            Assert.NotNull(videoFromDb);

            Assert.Equal(videoToUpdate.Description, videoFromDb.Description);
            Assert.Equal(videoToUpdate.Title, videoFromDb.Title);
            Assert.Equal(videoToUpdate.UserProfileId, videoFromDb.UserProfileId);
            Assert.Equal(videoToUpdate.DateCreated, videoFromDb.DateCreated);
            Assert.Equal(videoToUpdate.Url, videoFromDb.Url);
        }

        [Fact]
        public void Delete_Method_Removes_A_User()
        {
            // Arrange
            var testUserId = 99;
            var users = CreateTestUserProfiles(5);
            users[0].Id = testUserId;

            var repo = new InMemoryUserProfileRepository(users);
            var controller = new UserProfileController(repo);

            // Act
            controller.Delete(testUserId);

            // Assert
            var userFromDb = repo.InternalData.FirstOrDefault(p => p.Id == testUserId);
            Assert.Null(userFromDb);
        }

        private List<UserProfile> CreateTestUserProfiles(int count)
        {
            var users = new List<UserProfile>();
            for (var i = 1; i <= count; i++)
            {
                users.Add(new UserProfile()
                {
                    Id = i,
                    Name = $"{i} name",
                    Email = $"number {i} email",
                    DateCreated = DateTime.Today.AddDays(-i),
                    ImageUrl = $"number {i} image url"
                });
            }
            return users;
        }
    }
}