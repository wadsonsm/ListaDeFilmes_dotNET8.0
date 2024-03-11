using Filmes.Application.Context;
using Filmes.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesAsp8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _movieContext;
        public MoviesController(MovieContext movieContext)
        {
            _movieContext = movieContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            if (_movieContext == null)
                return BadRequest("Favor não enviar valores vazios");

            return await _movieContext.Movies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            if (_movieContext.Movies is null)
            {
                return NotFound();
            }

            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound();
            }
            return movie;
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            if (movie == null)
                return BadRequest("Tem nada aqui pra salvar");

            _movieContext.Movies.Add(movie);
            await _movieContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        [HttpPut]
        public async Task<ActionResult<Movie>> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }
            _movieContext.Entry(movie).State = EntityState.Modified;
            try
            {
                await _movieContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }               
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            if (_movieContext.Movies is null)
            {
                return NotFound();
            }
            var movie = await _movieContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound();
            }
            _movieContext.Movies.Remove(movie);
            await _movieContext.SaveChangesAsync();
            return NoContent();
        }

        private bool MovieExists(long id)
        {
            return (_movieContext.Movies?.Any(movie => movie.Id == id)).GetValueOrDefault();    
        }
    }
}
