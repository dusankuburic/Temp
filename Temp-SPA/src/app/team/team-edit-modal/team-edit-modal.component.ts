import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { takeUntil } from 'rxjs';
import { Team } from 'src/app/core/models/team';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { TeamService } from 'src/app/core/services/team.service';
import { TeamValidators } from '../team-validators';
import { DestroyableComponent } from 'src/app/core/base/destroyable.component';
import { BlobDto, BlobResponse } from 'src/app/core/models/blob';
import { faFile } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-team-edit-modal',
    templateUrl: './team-edit-modal.component.html',
    styleUrls: ['../../shared/styles/modal.css'],
    standalone: false
})
export class TeamEditModalComponent extends DestroyableComponent implements OnInit{
  fileIcon = faFile;

  editTeamForm!: FormGroup;
  team!: Team;

  groupId!: number;
  teamId!: number;
  title?: string;

  teamFiles: BlobDto[] = [];

  name = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(60)]);

  constructor(
    private teamService: TeamService,
    private fb: FormBuilder,
    private alertify: AlertifyService,
    private validators: TeamValidators,
    public bsModalRef: BsModalRef) {
      super();
    }

  ngOnInit(): void {
    this.editTeamForm = this.fb.group({
      name: this.name
    });

    this.teamService.getTeam(this.teamId).pipe(takeUntil(this.destroy$)).subscribe({
      next: (res: Team) => {
        this.team = res;
        this.setupForm(this.team);
      },
      error: () => {
        this.alertify.error('Unable to get team');
      }
    });
  }

  setupForm(team: Team) {
    this.editTeamForm.patchValue({
      name: team.name
    });

    this.name.addAsyncValidators(this.validators.validateNameNotTaken(this.groupId, team.name));
  }

  update(): void {
    const teamForm = { ...this.editTeamForm.value };
    this.team.name = teamForm.name;
    this.teamService.updateTeam(this.team.id, this.team).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.bsModalRef.content.isSaved = true;
        this.alertify.success('Successfully updated');
      },
      error: () => {
        this.alertify.error('Unable to update team');
      }
    });
  }

  onFileUploaded(response: BlobResponse): void {
    if (!response.error && response.blob) {
      this.teamFiles = [...this.teamFiles, response.blob];
    }
  }

  onFileDeleted(path: string): void {
    this.teamFiles = this.teamFiles.filter(f => f.name !== path);
  }
}
