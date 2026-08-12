using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.Exceptions;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Entities;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Library.BookService.Infrastructure.Adapters.Editors
{
    public class EditorRepositoryEF : IEditorRepositoryPort
    {
        private readonly EditorEntityMapper _editorMapper;
        private readonly BookDBContext _context;
        private readonly ILoggerPort _logger;

        public EditorRepositoryEF(
            EditorEntityMapper editorMapper,
            BookDBContext context,
            ILoggerPort logger)
        {
            _editorMapper = editorMapper;
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Editor> Items, int TotalRecords)> GetEditorsAsync(Editor searchEditor, int page, int pageSize)
        {
            _logger.Info($"GetEditorsAsync - Started | Editor name: {searchEditor.Name ?? "null"}");
            try
            {
                int offset = (page - 1) * pageSize;

                IQueryable<EditorEntity> query = _context.Editors;
                // questa operazione non è async perchè è come se stessi solo costruendo la query non interrogo 
                // il db e quindi non neccessito del comportamento async per non aspettare la risposta del db

                if (searchEditor.Id > 0)
                {
                    query = query.Where(e => e.Id == searchEditor.Id);
                }

                if (!string.IsNullOrEmpty(searchEditor.Name))
                {
                    query = query.Where(e => e.Name.Contains(searchEditor.Name));
                    // Where estrae tutti i risultati di una condizione il First solo la prima occorrenza.
                }

                int total = await query.CountAsync();

                var editorEntities = await query
                    .OrderBy(e => e.Name)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();
                // qui è il ToListAsync a chiamare subito il db mentre OrderBy, skip where o take costruiscono
                // solo la query. Un altro esempio come il ToListAsync per cui utiliziamo l'async con l'await
                // è il CountAsync

                _logger.Info($"GetEditorsAsync - Completed | {editorEntities.Count} items");

                return (_editorMapper.ToDomainList(editorEntities), total);

            }
            catch (Exception e)
            {
                _logger.Error($"GetEditorsAsync - Error", e);
                throw new EditorRepositoryEFException("Error retrieving editors", e);
            }
            //Il catch generico Exception cattura qualsiasi errore inaspettato(DB down, timeout, errore EF, ecc.) e lo wrappa in EditorRepositoryEFException con due scopi:
            //1 — Nascondere i dettagli interni — chi chiama il repository non deve sapere se l'errore viene da EF, MySQL o altro. Riceve sempre EditorRepositoryEFException — un'eccezione del tuo dominio.
            //2 — Preservare l'eccezione originale — new EditorRepositoryEFException("Error retrieving editors", e) — la e originale viene passata come innerException, quindi nei log hai tutta la catena:
            //EditorRepositoryEFException: Error retrieving editors
            //  → caused by: MySqlException: Connection refused
            //      → caused by: ...
            //Quindi la struttura è:
            //            EF / DB esplode(Exception qualsiasi)
            //  → catch (Exception e)
            //  → log
            //  → throw new EditorRepositoryEFException(..., e)  // wrappa mantenendo l'originale
            //  → il controller riceve EditorRepositoryEFException → 500
            //È il pattern repository exception wrapping — il repository traduce eccezioni infrastrutturali in eccezioni del tuo livello applicativo.
            // Per questo motivo hai prima Exception e dentro EditorRepositoryEFException perchè ad ogni Exception
            // scatenata poi sarà wrappata in EditorRepositoryEFException preservando sia l'eccezione originale
            // che il layer infrastrutturale dove viene scatenata. 
            // se avessi scritto catch (EditorRepositoryEFException e) allora non avrebbe catchato mai
            // nessuna eccezione.
        }

        public async Task<(Editor Editor, int TotalBooks)> GetEditorDetailAsync(long id, int page, int pageSize)
        {
            _logger.Info($"GetEditorDetailAsync - Started | Id: {id}");
            try
            {
                // async perchè sto già interrogando il db quando chiamo un metodo come FirstOrDefaultAsync
                var editorEntity = await _context.Editors.FirstOrDefaultAsync(e => e.Id == id);
                // prende il primo elemento che soddisfa la condizione se ne trova di più
                // prende semplicemente il primo e ignora gli altri.
                // Si differenzia da FirstAsync perchè gestisce il caso in cui non lo trova dando il default
                // se non lo trova da il valore default di oggetto che è null, mentre se avessi utilizzato
                // FirstAsync non trovandolo avrebbe lanciato un eccezione rompendo l'applicativo
                // che volendo avrei poi gestito.
                // Con il First o FirstOrDefault il primo record restituito lo deciderà il db,
                // quindi non hai garanzia su quale sarà il risultato ragion per cui, tranne in questo caso
                // dove l'id deve essere univoco viene utilizzato insieme all'order by in modo da garantire
                // un risultato aspettato secondo un ordinamento.

                if (editorEntity == null)
                {
                    _logger.Warn($"GetEditorDetailAsync - Editor not found | Id: {id}");
                    return (null, 0);
                }

                // Paginazione sui libri
                int totalBooks = await _context.Books
                    .Where(b => b.EditorId == id)
                    .CountAsync();

                int offset = (page - 1) * pageSize;

                var books = await _context.Books
                    .Where(b => b.EditorId == id)
                    .OrderBy(b => b.Title)
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                editorEntity.Books = books.ToList();

                _logger.Info($"GetEditorDetailAsync - Completed | Editor: {editorEntity.Name}, Books: {books.Count}");
                return (_editorMapper.ToDomain(editorEntity), totalBooks);
            }
            catch (Exception e)
            {
                _logger.Error($"GetEditorDetailAsync - Error", e);
                throw new EditorRepositoryEFException("Error retrieving editor by id", e);
            }
        }
        public async Task<Editor> CreateEditorAsync(Editor editor)
        {
            _logger.Info($"CreateEditorAsync - Start | Creation Editor: {editor.Name}");
            try
            {
                var editorEntity = new EditorEntity
                {
                    Name = editor.Name,
                };

                await _context.Editors.AddAsync(editorEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"Editor ID {editorEntity.Id} created");
                return _editorMapper.ToDomain(editorEntity);
            }
            catch (Exception e)
            {
                _logger.Error($"Error CreateEditorAsync for editor {editor.Name}", e);
                throw new EditorRepositoryEFException("Error creating editor: " + e.Message);

            }
        }
        public async Task<Editor?> GetEditorByIdAsync(long id)
        {
            _logger.Info($"GetEditorByIdAsync - Start | Id: {id}");

            try
            {
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (editorEntity == null)
                {
                    return null;
                }

                _logger.Info($"GetEditorByIdAsync - Completed | Id: {id}");
                return _editorMapper.ToDomain(editorEntity);
            }
            catch (Exception ex)
            {
                _logger.Error($"GetEditorByIdAsync - Error | Id: {id}", ex);
                throw new EditorRepositoryEFException("Error retrieving editor by id", ex);
            }
        }
        public async Task<Editor> UpdateEditorAsync(Editor editor)
        {
            _logger.Info($"UpdateEditorAsync - Started | Id: {editor.Id}");

            try
            {
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == editor.Id);

                if (editorEntity == null)
                {
                    _logger.Warn($"UpdateEditorAsync - Editor not found | Id: {editor.Id}");
                    throw new EditorRepositoryEFException($"Editor with id {editor.Id} not found");
                }

                editorEntity.Name = editor.Name;

                await _context.SaveChangesAsync();

                _logger.Info($"UpdateEditorAsync - Completed | Id: {editor.Id}");
                return _editorMapper.ToDomain(editorEntity);
            }
            catch (EditorRepositoryEFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateEditorAsync - Error | Id: {editor.Id}", ex);
                throw new EditorRepositoryEFException("Error updating editor", ex);
            }
        }
        //Caso 1 — Editor non trovato:
        //if (editorEntity == null)
        //    → throw new EditorRepositoryEFException("Editor not found")  // lanciata manualmente
        //    → catch (EditorRepositoryEFException) → throw  // rilancia identica
        //    → arriva al controller → BadRequest o NotFound

        //Caso 2 — Errore DB/EF:
        //await _context.SaveChangesAsync()  // esplode per errore DB
        //    → catch (Exception ex)  // cattura qualsiasi altra eccezione
        //    → log dell'errore
        //    → throw new EditorRepositoryEFException("Error updating editor", ex)  // wrappa
        //    → arriva al controller → StatusCode 500
        //Senza il primo catch (EditorRepositoryEFException), il caso 1 verrebbe catturato dal
        //secondo catch — perché EditorRepositoryEFException è una sottoclasse di Exception — e
        //otterresti un messaggio sbagliato tipo "Error updating editor" invece di "Editor with id X not found", perdendo il contesto.
        //Quindi la regola generale è: se lanci manualmente un'eccezione specifica dentro un try
        //che ha un catch generico, devi proteggere quella eccezione con un catch specifico prima di quello generico.
        // Il primo catch che vedi quindi si riferisce all'eccezione più interna mentre l'ultimo catch gestische
        //l'eccezione più esterna
        public async Task DeleteEditorAsync(long id)
        {
            _logger.Info($"DeleteEditorAsync - Start | Id: {id}");

            try
            {
                var editorEntity = await _context.Editors
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (editorEntity == null)
                {
                    _logger.Warn($"DeleteEditorAsync - Editor not found | Id: {id}");
                    throw new EditorRepositoryEFException($"Editor with id {id} not found");
                }

                // Controlla se ci sono libri associati
                var hasBooks = await _context.Books
                    .AnyAsync(b => b.EditorId == id);

                if (hasBooks)
                {
                    _logger.Warn($"DeleteEditorAsync - Editor has books | Id: {id}");
                    throw new EditorRepositoryEFException($"Cannot delete editor with id {id} because it has books associated. Please delete the books first.");
                }

                _context.Editors.Remove(editorEntity);
                await _context.SaveChangesAsync();

                _logger.Info($"DeleteEditorAsync - Completed | Id: {id}");
            }
            catch (EditorRepositoryEFException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteEditorAsync - Error | Id: {id}", ex);
                throw new EditorRepositoryEFException("Error deleting editor", ex);
            }
        }
    }
}
