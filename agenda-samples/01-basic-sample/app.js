const Agenda = require("agenda");

function time() {
    return new Date().toTimeString().split(' ')[0];
}

const agenda = new Agenda({
    db: {
        address: 'mongodb://root:admin123@localhost:27017',
        options: { useNewUrlParser: true },
        //collection: `agendaJobs-${Math.random()}` // Start fresh every time
        collection: `agendaJobs`
    }
});

agenda.define(
    'hello-world',
    async (job, done) => {
        console.log(`Hello World!`);
        done();
    }
);

(async function () {
    console.log(time(), 'Agenda started');

    // Log job start and completion/failure
    agenda.on('start', (job) => {
        console.log(time(), `Job <${job.attrs.name}> starting`);
    });
    agenda.on('success', (job) => {
        console.log(time(), `Job <${job.attrs.name}> succeeded`);
    });
    agenda.on('fail', (error, job) => {
        console.log(time(), `Job <${job.attrs.name}> failed:`, error);
    });

    await agenda.start();

    await agenda.every("3 seconds", "hello-world");
    // Alternatively, you could also do:
    // await agenda.every("*/3 * * * * *", "hello-world");
})();