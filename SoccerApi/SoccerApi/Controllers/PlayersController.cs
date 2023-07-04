﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoccerApi.Data;
using SoccerApi.Models.DTO;
using SoccerApi.Models;
using System.Numerics;
using System.Diagnostics.Metrics;

namespace SoccerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly SoccerDbContext dbContext;
        public PlayersController(SoccerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetAll()
        {

            var players = dbContext.Players.ToList();

            //Map Models to DTOs
            var playerDto = new List<PlayerDto>();
            foreach (var player in players)
            {
                playerDto.Add(new PlayerDto()
                {
                    Id = player.Id,
                    Name = player.Name,
                    Goal = player.Goal,
                    Assist = player.Assist,
                    Country = player.Country,
                    LeagueId = player.LeagueId,
                    TeamId = player.TeamId
                });
            }
            //Return Dto back to client
            return Ok(playerDto);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var player = dbContext.Players.Find(id);
            if (player == null)
            {
                return NotFound();
            }
            //Map domain model to Dto
            var playerDto = new PlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                Goal = player.Goal,
                Assist = player.Assist,
                Country = player.Country,
                LeagueId = player.LeagueId,
                TeamId = player.TeamId
            };

            return Ok(playerDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] PlayerRequest playerRequest)
        {
            //Map Dto to model
            var playerModel = new Player
            {

                Name = playerRequest.Name,
                Goal = playerRequest.Goal,
                Assist = playerRequest.Assist,
                Country = playerRequest.Country,
                LeagueId = playerRequest.LeagueId,
                TeamId = playerRequest.TeamId
            };

            //Use model to create League, add to database
            dbContext.Players.Add(playerModel);
            dbContext.SaveChanges();

            //Map model back to Dto
            var playerDto = new PlayerDto
            {
                Id = playerModel.Id,
                Name = playerModel.Name,
                Goal = playerModel.Goal,
                Assist = playerModel.Assist,
                Country = playerModel.Country,
                LeagueId = playerModel.LeagueId,
                TeamId = playerModel.TeamId

            };
            //Returns 201 after creation, sends the new Id of the item just created
            return CreatedAtAction(nameof(GetById), new { id = playerModel.Id }, playerDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var player = dbContext.Players.Find(id);
            if (player != null)
            {
                dbContext.Remove(player);
                dbContext.SaveChanges();
                return Ok(player);
            }
            return NotFound();
        }


        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult Update([FromRoute] Guid id, PlayerRequest playerRequest)
        {
            //Try to find the id in the database
            var player = dbContext.Players.Find(id);
            if (player != null)
            {
                player.Assist = playerRequest.Assist;
                player.Name = playerRequest.Name;
                player.TeamId = playerRequest.TeamId;
                player.LeagueId = playerRequest.LeagueId;
                player.Country = playerRequest.Country;
                player.Goal = playerRequest.Goal;

                        
                //save the database after making changes
                dbContext.SaveChanges();
                return Ok(player);
            }

            //return saying the id is not found in the database
            return NotFound();
        }
    }
}
