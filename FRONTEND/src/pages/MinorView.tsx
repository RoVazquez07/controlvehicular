import { Ibox } from '../components/Ibox'

const rows = [
  { project: 'Project', task: '20%', date: 'Jul 14, 2013' },
  { project: 'Alpha project', task: '40%', date: 'Jul 16, 2013' },
  { project: 'Betha project', task: '75%', date: 'Jul 18, 2013' },
  { project: 'Gamma project', task: '18%', date: 'Jul 22, 2013' },
]

export function MinorView() {
  return (
    <div className="wrapper wrapper-content animated fadeInRight">
      <div className="row">
        <div className="col-lg-12">
          <div className="text-center m-t-lg m-b-lg">
            <h1>Simple examples</h1>
            <small>Few basic examples</small>
          </div>
        </div>
        <div className="col-lg-4">
          <Ibox title="Panel example">
            <p>
              Lorem Ipsum is simply dummy text of the printing and typesetting
              industry. Lorem Ipsum has been the industry&apos;s standard dummy
              text ever since the 1500s, when an unknown printer took a galley
              of type and scrambled it to make a type specimen book. It has
              survived not only five centuries, but also the leap into
              electronic typesetting, remaining essentially unchanged.
            </p>
          </Ibox>
        </div>
        <div className="col-lg-8">
          <Ibox title="Table example">
            <div className="table-responsive">
              <table className="table table-striped">
                <thead>
                  <tr>
                    <th>Project </th>
                    <th>Task</th>
                    <th>Date</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row) => (
                    <tr key={row.project}>
                      <td>{row.project}</td>
                      <td>{row.task}</td>
                      <td>{row.date}</td>
                      <td>
                        <a href="#">
                          <i className="fa fa-check text-navy"></i>
                        </a>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Ibox>
        </div>
      </div>
    </div>
  )
}
